using Consumer;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace tests
{
    public class ConsumerPactTests : IClassFixture<ConsumerPactClassFixture>
    {
        private IMockProviderService _mockProviderService;
        private string _mockProviderServiceBaseUri; 

        public ConsumerPactTests(ConsumerPactClassFixture fixture)
        {
            _mockProviderService = fixture.MockProviderService;
            _mockProviderService.ClearInteractions(); // clears previously registered interactions before the test is run
            _mockProviderServiceBaseUri = fixture.MockProviderServiceBaseUri;
        }

        [Fact]
        public void ItHandlesInvalidDateParameter()
        {
            var invalidRequestMessage = "validDateTime is not a date or time";
            _mockProviderService.Given("There is data")
                                .UponReceiving("An invalid date parameter in GET request")
                                .With(new ProviderServiceRequest {
                                    Method = HttpVerb.Get,
                                    Path = "/api/provider",
                                    Query = "validDateTime=sth"
                                })
                                .WillRespondWith(new ProviderServiceResponse { 
                                    Status = 400,
                                    Headers = new Dictionary<string, object>
                                    {
                                        { "Content-Type", "application/json; charset = utf-8"}
                                    },
                                    Body = new { message = invalidRequestMessage}
                                });

            var result = ConsumerApiClient.ValidateDateTimeUsingProviderApi("sth", _mockProviderServiceBaseUri).GetAwaiter().GetResult();
            var resultBodyTest = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            Assert.Contains(invalidRequestMessage, resultBodyTest);
        }

    }
}
