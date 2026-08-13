using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Diginsight;
using Diginsight.AspNetCore;
using Diginsight.Components;
using Diginsight.Components.Configuration;
using Diginsight.Diagnostics;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Learn.Web;

public class Program
{
    private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new();

    public static void Main(string[] args)
    {
        using var observabilityManager = new ObservabilityManager();
        ILogger logger = observabilityManager.LoggerFactory.CreateLogger(typeof(Program));

        WebApplication app;
        using (var activity = Observability.ActivitySource.StartMethodActivity(logger, new { args }))
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.ConfigureAppConfiguration2(observabilityManager.LoggerFactory);

            IServiceCollection services = builder.Services;
            IConfiguration configuration = builder.Configuration;
            IWebHostEnvironment environment = builder.Environment;

            // Diginsight telemetry integrated with OpenTelemetry.
            services.AddAspNetCoreObservability(configuration, environment, out IOpenTelemetryOptions openTelemetryOptions);
            observabilityManager.AttachTo(services);
            services.AddHttpObservability(openTelemetryOptions);

            services.TryAddSingleton<EarlyLoggingManager>(observabilityManager);
            services.AddHttpContextAccessor();
            services.AddDynamicLogLevel<DefaultDynamicLogLevelInjector>();

            // Bind the blob-proxy options: target storage account + cache/invalidation settings.
            // Bound via IOptions so environment overlays (e.g. the Testmc external
            // appsettings.Testmc.json merged by ConfigureAppConfiguration2) are honored.
            services.Configure<BlobProxyOptions>(configuration.GetSection("BlobProxy"));

            // In-memory LRU cache with a size cap (bytes) so memory stays bounded. Read the cap
            // from the current configuration (local appsettings); DI-resolved options below carry
            // any environment-specific overrides for the storage target.
            long cacheSizeLimit = configuration.GetValue("BlobProxy:CacheSizeLimitBytes", 200_000_000L);
            services.AddMemoryCache(o => o.SizeLimit = cacheSizeLimit);

            // A single BlobContainerClient bound to the configured storage account/container.
            // Options are resolved from DI (not a startup snapshot) so external environment
            // configuration (Testmc ΓåÆ samplestmcstitn01) is applied correctly.
            //
            // Authentication reuses Diginsight's DefaultCredentialProvider ΓÇö the same chain the
            // Key Vault integration uses. In Development it chains Azure CLI ΓåÆ VS Code ΓåÆ Visual
            // Studio (plus a client secret/certificate if configured in the credential section),
            // so it works from a developer machine with no IMDS endpoint. In Azure it falls back
            // to Workload/Managed Identity. This avoids the raw DefaultAzureCredential managed-
            // identity probe that hard-fails locally (169.254.169.254 unreachable).
            services.AddSingleton(sp =>
            {
                BlobProxyOptions opts = sp.GetRequiredService<IOptions<BlobProxyOptions>>().Value;
                IConfiguration config = sp.GetRequiredService<IConfiguration>();
                IHostEnvironment env = sp.GetRequiredService<IHostEnvironment>();
                ILogger credentialLogger =
                    sp.GetRequiredService<ILoggerFactory>().CreateLogger<DefaultCredentialProvider>();

                var credentialProvider = new DefaultCredentialProvider(env, credentialLogger);
                // Reuse the same credential configuration section as Azure Key Vault so the proxy
                // authenticates with the developer's Azure CLI / Visual Studio login locally.
                TokenCredential credential = credentialProvider.Get(config.GetSection("BlobProxy"));

                var accountUri = new Uri(opts.AccountUri.TrimEnd('/') + "/");
                var containerUri = new Uri(accountUri, opts.ContainerName);
                return new BlobContainerClient(containerUri, credential);
            });

            builder.UseDiginsightServiceProvider(true);

            app = builder.Build();
            logger.LogDebug("Host built");

            // Log the effective (post-merge) storage target so the active environment is visible.
            BlobProxyOptions effective = app.Services.GetRequiredService<IOptions<BlobProxyOptions>>().Value;
            logger.LogInformation("Blob-proxy target: {AccountUri} container '{Container}'", effective.AccountUri, effective.ContainerName);

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            // After startup, register the mapping to the configured storage account, as described
            // in the analysis article: each request path maps to a blob name, the blob is fetched
            // over HTTPS via the SDK, cached in memory, and returned with the correct content type.
            // A deploy uploads changed blobs, then POSTs /_cache/invalidate for instant refresh.
            MapBlobProxy(app);
        }

        app.Run();
    }

    private static void MapBlobProxy(WebApplication app)
    {
        using var observabilityManager = new ObservabilityManager();
        ILogger logger = observabilityManager.LoggerFactory.CreateLogger(typeof(Program));

        // Cache-invalidation endpoint. A deploy calls this after uploading changed blobs so the
        // next request serves fresh content. Optionally protected by a shared-secret header.
        app.MapPost("/_cache/invalidate", (
            string? path,
            HttpContext http,
            IMemoryCache cache,
            IOptions<BlobProxyOptions> options,
            ILoggerFactory loggerFactory) =>
        {
            ILogger logger = loggerFactory.CreateLogger("Learn.Web.CacheInvalidate");
            using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { path });

            BlobProxyOptions opts = options.Value;
            if (!IsInvalidateAuthorized(http, opts))
            {
                return Results.Unauthorized();
            }

            if (string.IsNullOrEmpty(path))
            {
                // IMemoryCache exposes no key enumeration; MemoryCache.Clear() flushes everything.
                if (cache is MemoryCache memoryCache)
                {
                    memoryCache.Clear();
                }
                logger.LogInformation("Cache flushed (all entries)");
                return Results.Ok(new { flushed = "all" });
            }

            string key = Normalize(path);
            cache.Remove(key);
            logger.LogInformation("Cache entry invalidated: {Key}", key);
            return Results.Ok(new { flushed = key });
        });

        // Catch-all GET: forward every request to the configured storage account.
        app.MapGet("/{**path}", async (
            string? path,
            HttpContext http,
            IMemoryCache cache,
            BlobContainerClient container,
            IOptions<BlobProxyOptions> options,
            ILoggerFactory loggerFactory) =>
        {
            ILogger logger = loggerFactory.CreateLogger("Learn.Web.BlobProxy");
            string key = Normalize(path);
            using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { key });

            BlobProxyOptions opts = options.Value;

            CachedContent entry = (await cache.GetOrCreateAsync(key, async cacheEntry =>
            {
                var blob = container.GetBlobClient(key);
                if (!await blob.ExistsAsync())
                {
                    var notFound = container.GetBlobClient(opts.NotFoundBlob);
                    if (await notFound.ExistsAsync())
                    {
                        var nf = await notFound.DownloadContentAsync();
                        byte[] nfBytes = nf.Value.Content.ToArray();
                        cacheEntry.SetSize(Math.Max(1, nfBytes.Length));
                        return new CachedContent(nfBytes, "text/html; charset=utf-8", StatusCodes.Status404NotFound);
                    }

                    cacheEntry.SetSize(1);
                    return new CachedContent([], "text/plain; charset=utf-8", StatusCodes.Status404NotFound);
                }

                var response = await blob.DownloadContentAsync();
                byte[] bytes = response.Value.Content.ToArray();
                cacheEntry.SetSize(Math.Max(1, bytes.Length));

                string contentType = response.Value.Details.ContentType is { Length: > 0 } stored
                    ? stored
                    : ContentTypeProvider.TryGetContentType(key, out string? mime) && mime is not null
                        ? mime
                        : "application/octet-stream";

                return new CachedContent(bytes, contentType, StatusCodes.Status200OK);
            }))!;

            await WriteResponseAsync(http, entry);
        });
    }

    /// <summary>Writes the cached blob to the response, with ETag revalidation for HTML.</summary>
    private static async Task WriteResponseAsync(HttpContext http, CachedContent entry)
    {
        bool isHtml = entry.ContentType.StartsWith("text/html", StringComparison.OrdinalIgnoreCase);

        if (isHtml && entry.Status == StatusCodes.Status200OK)
        {
            // Long-lived origin content: let clients revalidate so an invalidation is picked up.
            http.Response.Headers.CacheControl = "no-cache";
            http.Response.Headers.ETag = entry.ETag;

            if (http.Request.Headers.IfNoneMatch.Count > 0 &&
                http.Request.Headers.IfNoneMatch.ToString().Contains(entry.ETag, StringComparison.Ordinal))
            {
                http.Response.StatusCode = StatusCodes.Status304NotModified;
                return;
            }
        }

        http.Response.StatusCode = entry.Status;
        http.Response.ContentType = entry.ContentType;
        http.Response.ContentLength = entry.Bytes.Length;
        if (entry.Bytes.Length > 0)
        {
            await http.Response.Body.WriteAsync(entry.Bytes);
        }
    }

    private static bool IsInvalidateAuthorized(HttpContext http, BlobProxyOptions options)
    {
        if (string.IsNullOrEmpty(options.InvalidateApiKey))
        {
            // No key configured: rely on Easy Auth / network restrictions in front of the app.
            return true;
        }

        return http.Request.Headers.TryGetValue("X-Invalidate-Key", out var provided) && 
            CryptographicOperations.FixedTimeEquals(System.Text.Encoding.UTF8.GetBytes(provided.ToString()), System.Text.Encoding.UTF8.GetBytes(options.InvalidateApiKey));
    }

    /// <summary>
    /// Maps a request path to a blob name: "" ΓåÆ index.html, "foo/" or extensionless "foo" ΓåÆ
    /// foo/index.html, otherwise the path is used verbatim.
    /// </summary>
    private static string Normalize(string? path)
    {
        string trimmed = (path ?? string.Empty).Trim('/');
        if (trimmed.Length == 0)
        {
            return "index.html";
        }

        bool endsWithSlash = (path ?? string.Empty).EndsWith('/');
        string lastSegment = trimmed.Contains('/') ? trimmed[(trimmed.LastIndexOf('/') + 1)..] : trimmed;
        if (endsWithSlash || !lastSegment.Contains('.'))
        {
            return trimmed + "/index.html";
        }

        return trimmed;
    }

    private sealed record CachedContent
    {
        public CachedContent(byte[] bytes, string contentType, int status)
        {
            Bytes = bytes;
            ContentType = contentType;
            Status = status;
            ETag = "\"" + Convert.ToHexString(SHA1.HashData(bytes)) + "\"";
        }

        public byte[] Bytes { get; }
        public string ContentType { get; }
        public int Status { get; }
        public string ETag { get; }
    }
}
