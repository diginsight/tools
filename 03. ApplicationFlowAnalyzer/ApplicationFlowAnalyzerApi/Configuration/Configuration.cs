using Azure.Core;
using Azure.Identity;
using Diginsight.Diagnostics;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace DiginsightCopilotApi.Configuration;

public static class HostingExtensions
{
    public static IHostBuilder ConfigureAppConfigurationNH(this IHostBuilder hostBuilder)
    {
        var logger = Program.LoggerFactory.CreateLogger(typeof(AddObservabilityExtension));
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { hostBuilder });

        return hostBuilder.ConfigureAppConfiguration(
            (hbc, cb) => ConfigureAppConfigurationNH(hbc.HostingEnvironment, cb)
        );
    }

    public static void ConfigureAppConfigurationNH(IHostEnvironment environment, IConfigurationBuilder builder)
    {
        var logger = Program.LoggerFactory.CreateLogger(typeof(AddObservabilityExtension));
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { environment, builder });

        bool isLocal = environment.IsDevelopment();

        string appsettingsEnvName = Environment.GetEnvironmentVariable("AppsettingsEnvironmentName") ?? environment.EnvironmentName;
        bool isChina = appsettingsEnvName.EndsWith("cn", StringComparison.OrdinalIgnoreCase);

        var configuration = builder.Build();

        const string prefix = "AzureKeyVault:";
        string? kvUri = configuration[$"{prefix}Uri"];
        if (string.IsNullOrEmpty(kvUri)) { return; }

        var credential = default(TokenCredential);
        string? tenantId = HardTrim(configuration[$"{prefix}TenantId"]);
        string? clientId = HardTrim(configuration[$"{prefix}ClientId"]);
        string? clientSecret = HardTrim(configuration[$"{prefix}ClientSecret"]);
        if (tenantId is not null && clientId is not null && clientSecret is not null)
        {
            Console.WriteLine("Using ClientSecretCredential");
            ClientSecretCredentialOptions credentialOptions = new();
            if (isChina) { credentialOptions.AuthorityHost = AzureAuthorityHosts.AzureChina; }
            credential = new ClientSecretCredential(tenantId, clientId, clientSecret, credentialOptions);
        }
        else if (!isLocal)
        {
            Console.WriteLine("Using ManagedIdentityCredential");
            credential = new ManagedIdentityCredential();
        }
        else if (isLocal)
        {
            Console.WriteLine("Using DefaultAzureCredential");
            AzureCliCredentialOptions credentialOptions = new();
            //if (isChina) { credentialOptions.AuthorityHost = AzureAuthorityHosts.AzureChina; }
            credential = new DefaultAzureCredential(true);
        }

        builder.AddAzureKeyVault(new Uri(kvUri), credential);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static string? HardTrim(string? s) => s?.Trim() is not "" and var s0 ? s0 : null;
}
