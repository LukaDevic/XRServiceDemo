using Newtonsoft.Json;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;
using XRServiceWeb.Tests.Base;
using Xunit;
using Xunit.Abstractions;

namespace XRServiceWeb.Tests.HistoricalXR
{
    public class HistoricalXRControllerShould : IClassFixture<TestFixture>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
        private const string BaseUrl = "/api/historicalxr";

        public HistoricalXRControllerShould(TestFixture fixture, ITestOutputHelper output)
        {
            _client = fixture.CreateClient();
            _output = output;
        }

        [Fact]
        public async Task HasReturnedHistoricalExchangeRatesWithParametersGiven()
        {
            var testMinimumRate = 0.9014122137;
            var testMaximumRate = 1.1070586641;
            var testAverageRate = 0.9360668399654967;

            var urlParams = "?basecurrency=SEK&dates=2018-04-08&dates=2020-04-08&targetcurrency=NOK";
            var requestUrl = $"{BaseUrl}{urlParams}";

            var response = await _client.GetAsync(requestUrl);
            await response.EnsureSuccessStatusCode(_output);

            var dataAsString = await response.Content.ReadAsStringAsync();
            dynamic content = JsonConvert.DeserializeObject<ExpandoObject>(dataAsString);

            Assert.True(content.minimumRate.rate == testMinimumRate);
            Assert.True(content.maximumRate.rate == testMaximumRate);
            Assert.True(content.averageRate == testAverageRate);
        }
    }
}
