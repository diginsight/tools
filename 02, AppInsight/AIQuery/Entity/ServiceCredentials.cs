using Microsoft.Identity.Client;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationInsightQuery
{
    public class BearerTokenClientCredentials : ServiceClientCredentials
    {
        private string token;

        public BearerTokenClientCredentials(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException($"{nameof(token)} must not be null or empty");
            }

            this.token = token;
        }

        public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return Task.FromResult(true);
        }
    }
    //public class CustomLoginCredentials : ServiceClientCredentials
    //{
    //    private string AuthenticationToken { get; set; }
    //    public override void InitializeServiceClient<T>(ServiceClient<T> client)
    //    {
    //        var authenticationContext = new AuthenticationContext("https://login.windows.net/{tenantID}");
    //        var credential = new ClientCredential(clientId: "xxxxx-xxxx-xx-xxxx-xxx", clientSecret: "{clientSecret}");

    //        var result = authenticationContext.AcquireToken(resource: "https://management.core.windows.net/", clientCredential: credential);

    //        if (result == null) throw new InvalidOperationException("Failed to obtain the JWT token");

    //        AuthenticationToken = result.AccessToken;
    //    }
    //    public override async Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    //    {
    //        if (request == null) throw new ArgumentNullException("request");

    //        if (AuthenticationToken == null) throw new InvalidOperationException("Token Provider Cannot Be Null");

    //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticationToken);
    //        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    //        //request.Version = new Version(apiVersion);
    //        await base.ProcessHttpRequestAsync(request, cancellationToken);
    //    }
    //}
}
