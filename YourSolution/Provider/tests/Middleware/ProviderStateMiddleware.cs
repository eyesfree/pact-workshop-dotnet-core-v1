using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace tests.Middleware
{
    public class ProviderStateMiddleware
    {
        private const string ConsumerName = "Consumer";
        private const string COMMON_PACT_FOLDER = @"../../../../../data";
        private const string COMMON_FILE_NAME = "somedata.txt";
        private readonly RequestDelegate _requestDelegate;
        private readonly IDictionary<string, Action> _providerStates;

        public ProviderStateMiddleware(RequestDelegate next)
        {
            _requestDelegate = next;
            _providerStates = new Dictionary<string, Action>
            {
                { "There is data", AddData},
                { "There is no data", RemoveAllData }
            };
        }

        private static void RemoveAllData()
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), COMMON_PACT_FOLDER);
            var deletePath = Path.Combine(folderPath, COMMON_FILE_NAME);

            if (File.Exists(deletePath))
            {
                File.Delete(deletePath);
            }
        }

        private static void AddData()
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), COMMON_PACT_FOLDER);
            var createPath = Path.Combine(folderPath, COMMON_FILE_NAME);

            if (!File.Exists(createPath))
            {
                File.Create(createPath);
            }
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path == "/provider-states")
            {
                this.HandleProviderStatesRequest(httpContext);
                await httpContext.Response.WriteAsync(String.Empty);
            }
            else
            {
                await this._requestDelegate(httpContext);
            }
        }

        private void HandleProviderStatesRequest(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = (int) HttpStatusCode.OK;
            if (httpContext.Request.Method.ToUpper() == HttpMethod.Post.ToString().ToUpper() && httpContext.Request.Body != null)
            {
                string jsonRequestBody = String.Empty;
                using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8))
                {
                    try
                    {
                        jsonRequestBody = reader.ReadToEnd();
                    }
                    catch {
                        Console.WriteLine("nothing to read");
                    }
                }

                var providerState = JsonConvert.DeserializeObject<ProviderState>(jsonRequestBody);

                if (providerState != null && !String.IsNullOrEmpty(providerState.State) && providerState.Consumer == ConsumerName)
                {
                    _providerStates[providerState.State].Invoke();
                }
            }
        }
    }
}
