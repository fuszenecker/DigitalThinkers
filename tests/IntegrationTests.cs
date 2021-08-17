using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;

namespace tests
{
    [TestClass]
    public class IntegrationTests
    {
        const string serviceBaseUrl = "http://api:5000";
        const string HealthCheckEndpointPath = "/health";
        const string MetricsEndpointPath = "/metrics";

        HttpClient httpClient = new HttpClient();

        public IntegrationTests()
        {
        }

        [TestMethod]
        public async Task HealthCheckWorks()
        {
            var result = await httpClient.GetAsync(serviceBaseUrl + HealthCheckEndpointPath);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual("Healthy", await result.Content.ReadAsStringAsync());
        }

        [TestMethod]
        public async Task MetricsWorks()
        {
            var result = await httpClient.GetAsync(serviceBaseUrl + MetricsEndpointPath);
            var responseBody = await result.Content.ReadAsStringAsync();

            Assert.AreEqual(System.Net.HttpStatusCode.OK, result.StatusCode);

            // Response body contains at least 20 lines:
            Assert.IsTrue(responseBody.Split('\n').Length > 20);
        }
    }
}
