using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Web;
using System.Text;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text.Json;

namespace azure_iot_dps_timing
{
    enum State {
        Unknown,
        Registering,
        Registered
    }

    class Program
    {
        private static String _Key = "";
        private static String _RegistrationId = "";
        private static String _ScopeId = "";
        private static String _Endpoint = "global.azure-devices-provisioning.net";
        private static String _ApiVersion = "2021-06-01";
        private static string _State = "";
        private static string _OperationId = "";


        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddHttpClient();

            var serviceProvider = services.BuildServiceProvider();
            var client = serviceProvider.GetService<HttpClient>();
            var sas = GenerateSas($"{_ScopeId}/registrations/{_RegistrationId}", "registration", _Key);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SharedAccessSignature", sas);

            var overallStopwatch = new Stopwatch();
            while(true) {
                if (_State == "" || _State == "registered") {
                    Console.Out.WriteLine("--------------------------------");
                    overallStopwatch.Start();

                    var sw = new Stopwatch();
                    sw.Start();
                    
                    var result = await Register(client);
                    sw.Stop();
                    Console.WriteLine($"Initial registration call for {result.operationId} completed in {TsFormat(sw.Elapsed)}");

                    _State = result == null ? "" : result.status;
                    _OperationId = result == null ? "" : result.operationId;
                } else if (_State == "assigning") {
                    await Task.Delay(1000);

                    var sw = new Stopwatch();
                    sw.Start();
                    var result = await GetStatus(client, _OperationId);
                    sw.Stop();
                    Console.WriteLine($"Registration Status check for {result.operationId} completed in {TsFormat(sw.Elapsed)}");

                    if(result.status == "assigned") {
                        Console.WriteLine($"Assignment made to {result.registrationState.assignedHub}.");
                    }

                    _State = result == null ? "" : result.status;
                } else if (_State == "assigned") {
                    Console.WriteLine($"Final assignment completed in {TsFormat(overallStopwatch.Elapsed)}");
                    overallStopwatch.Stop();
                    overallStopwatch.Reset();
                    Console.Out.WriteLine("--------------------------------");
                    _State = "";
                    await Task.Delay(5000);
                } else {
                    Console.WriteLine("oops!");
                }
            }
        }

        private static string GenerateSas(string resourceUri, string keyName, string key)
        {
            TimeSpan sinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var week = 60 * 60 * 24 * 7;
            var expiry = Convert.ToString((int)sinceEpoch.TotalSeconds + week);
            string stringToSign = Uri.EscapeDataString(resourceUri) + "\n" + expiry;
            
            HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String(key));
            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
            var sasToken = String.Format(CultureInfo.InvariantCulture, "sr={0}&sig={1}&se={2}&skn={3}", Uri.EscapeDataString(resourceUri), Uri.EscapeDataString(signature), expiry, keyName);
            return sasToken;
        }


        public static async Task<RegistrationOperationStatusResult> Register(HttpClient client) {
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = _Endpoint;
            uriBuilder.Path = $"{_ScopeId}/registrations/{_RegistrationId}/register";
            uriBuilder.Query = $"api-version={_ApiVersion}";

            var registrationRequest = new DeviceRegistrationRequest() {
                registrationId = _RegistrationId
            };

            
            var uri = uriBuilder.ToString();

            var response = await client.PutAsJsonAsync<DeviceRegistrationRequest>(
                uri,
                registrationRequest
            );

            if (response.IsSuccessStatusCode) {
                return await response.Content.ReadFromJsonAsync<RegistrationOperationStatusResult>();
            } else {
                var err = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error {err}");
                return null;
            }
        }

        public static string TsFormat(TimeSpan ts) {
            return String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10
            );
        }
        public static async Task<RegistrationOperationStatusResult> GetStatus(HttpClient client, string OperationId) {
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = _Endpoint;
            uriBuilder.Path = $"{_ScopeId}/registrations/{_RegistrationId}/operations/{OperationId}";
            uriBuilder.Query = $"api-version={_ApiVersion}";

            var registrationRequest = new DeviceRegistrationRequest() {
                registrationId = _RegistrationId
            };

            var uri = uriBuilder.ToString();

            var response = await client.GetAsync(
                uri
            );

            if (response.IsSuccessStatusCode) {
                var res =  await response.Content.ReadFromJsonAsync<RegistrationOperationStatusResult>();
                return res;
            } else {
                var err = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error {err}");
                return null;
            }

        }
    }
}
