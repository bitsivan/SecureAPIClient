using System.Net.Http.Headers;
using Microsoft.Identity.Client;

namespace SecureAPIClient
{
    public static class ConfigStartAD
    {
        public static async Task RunAsync()
        {
            AuthConfig config = AuthConfig.ReadFromJsonFile("appsettings.json");

            IConfidentialClientApplication app;

            app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
                    .WithClientSecret(config.ClientSecret)
                    .WithAuthority(new Uri(config.Authority))
                    .Build();

            string[] ResourceIds = new string[] { config.ResourceId };

            AuthenticationResult result = null;
            try
            {
                result = await app.AcquireTokenForClient(ResourceIds).ExecuteAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Token acquired \n");
                Console.WriteLine(result.AccessToken);

                if (!string.IsNullOrEmpty(result.AccessToken))
                {
                    var httpClient = new HttpClient();
                    var defaultRequestHeaders = httpClient.DefaultRequestHeaders;

                    // if (defaultRequestHeaders.Accept == null || defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json"))
                    // {
                    //     httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // }
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                    defaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.AccessToken);
                    HttpResponseMessage response = await httpClient.GetAsync(config.BaseAddress);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        string json = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(json);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Failed to call the web api: {response.StatusCode}");
                        string content = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Content:{content}");
                    }
                    Console.ResetColor();
                }
            }
            catch (MsalClientException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }
    }
}