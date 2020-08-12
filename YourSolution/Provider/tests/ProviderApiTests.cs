using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace tests
{
    public class ProviderApiTests : IDisposable
    {
        private string _providerUri { get; }
        private string _pactServiceUri { get; }

        private IWebHost _webHost { get; }

        private ITestOutputHelper _testOutputHelper { get; }

        public ProviderApiTests(ITestOutputHelper output)
        {
            _testOutputHelper = output;
            _providerUri = "http://localhost:9000";
            _pactServiceUri = "http://localhost:9001";
            _webHost = WebHost.CreateDefaultBuilder().UseUrls(_pactServiceUri).UseStartup<TestStartup>().Build();

            _webHost.Start();
        }

        [Fact]
        public void EnsureProviderApiResponseMatchesConsumerExpectation()
        {
            var config = new PactVerifierConfig
            {
                Outputters = new List<IOutput>
                {   new XUnitOutput(_testOutputHelper) },
                Verbose = true
            };

            IPactVerifier verifier = new PactVerifier(config);
            verifier
                .ProviderState($"{_pactServiceUri}/provider-states")
                .ServiceProvider("Provider", _providerUri)
                .HonoursPactWith("Consumer")
                .PactUri(@"../../../../../pacts/consumer-provider.json")
                .Verify();
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _webHost.StopAsync().GetAwaiter().GetResult();
                    _webHost.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

    }
}
