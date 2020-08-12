using PactNet;
using PactNet.Mocks.MockHttpService;
using System;

namespace tests
{
    // Shared mock server for Pact used by all the tests.
    public class ConsumerPactClassFixture : IDisposable
    {
        public IPactBuilder PactBuilder { get; set; }
        public IMockProviderService MockProviderService { get; private set; }

        public int MockServerPort => 9222;
        public string MockProviderServiceBaseUri => $"http://localhost:{MockServerPort}";

        public ConsumerPactClassFixture()
        {
            var pactConfig = new PactConfig
            {
                SpecificationVersion = "2.0.0",
                PactDir = @"..\..\..\..\..\pacts",
                LogDir = @".\pact\logs"
            };

            PactBuilder = new PactBuilder(pactConfig);
            PactBuilder.ServiceConsumer("Consumer").HasPactWith("Provider");

            MockProviderService = PactBuilder.MockService(MockServerPort);
        }


        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                PactBuilder.Build();
            }

            _disposedValue = true;
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
