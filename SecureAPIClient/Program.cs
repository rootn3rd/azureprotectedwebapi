using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace SecureAPIClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Calling the AAD");
            RunAsync().GetAwaiter().GetResult();

        }

        private static async Task RunAsync()
        {
            var config = AuthConfig.ReadJsonFromFile("appsettings.json");

            var app = ConfidentialClientApplicationBuilder
                .Create(config.ClientId)
                .WithClientSecret(config.ClientSecret)
                .WithAuthority(config.Authority)
                .Build();

            var resources = new string[] { config.ResourceId };
            AuthenticationResult result = null;
            try
            {
                result = await app.AcquireTokenForClient(resources).ExecuteAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Token Acquired :\n\n{result.AccessToken}");
            }
            catch (MsalClientException exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception.Message);
                Console.ResetColor();
            }


            if (!string.IsNullOrEmpty(result.AccessToken))
            {
                var httpClient = new HttpClient();
                var defaultRequestHeaders = httpClient.DefaultRequestHeaders;

                if (defaultRequestHeaders.Accept == null
                || !defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json"))
                {
                    httpClient
                        .DefaultRequestHeaders
                        .Accept
                        .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }

                defaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

                var response = await httpClient.GetAsync(config.BaseAddress);

                if (response.IsSuccessStatusCode)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response:\n\n{json}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Failed to call API - {response.StatusCode}");
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Content : {content}");
                    Console.ResetColor();
                }

            }

        }
    }
}
