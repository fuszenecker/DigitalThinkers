using ServiceTemplate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class IntegrationTests
    {
        private const string ServiceBaseUrl = "http://api:5000";
        private const string HealthCheckEndpointPath = "/health";
        private const string MetricsEndpointPath = "/metrics";
        private const string CorrelationIdHeaderName = "X-Correlation-ID";

        private HttpClient httpClient = null!;
        private Client serviceClient = null!;

        [TestInitialize]
        public void Initialize()
        {
            httpClient = new HttpClient() {
                BaseAddress = new Uri(ServiceBaseUrl)
            };

            serviceClient = new Client(ServiceBaseUrl, httpClient);
        }

        [TestMethod]
        public async Task HealthCheckWorks()
        {
            var result = await httpClient.GetAsync(HealthCheckEndpointPath).ConfigureAwait(false);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual("Healthy", await result.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        [TestMethod]
        public async Task MetricsWorks()
        {
            var result = await httpClient.GetAsync(MetricsEndpointPath).ConfigureAwait(false);
            var responseBody = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, result.StatusCode);

            // Response body contains at least 20 lines:
            Assert.IsTrue(responseBody.Split('\n').Length > 20);
        }

        [TestMethod]
        public async Task CorrelationIdWorks()
        {
            var correlationId = Guid.NewGuid().ToString();

            httpClient.DefaultRequestHeaders.Add(CorrelationIdHeaderName, correlationId);

            var result = await httpClient.GetAsync(MetricsEndpointPath).ConfigureAwait(false);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(correlationId, result.Headers.GetValues("X-Correlation-ID").First());
        }

        [TestMethod]
        [Ignore("When tests are run multiple times, this one will fail.")]
        public async Task GetStock()
        {
            var serviceClient = new Client(ServiceBaseUrl, new HttpClient { BaseAddress = new Uri(ServiceBaseUrl) });
            var stock = await serviceClient.StockGETAsync().ConfigureAwait(false);
            Assert.AreEqual(0, stock.Count);
        }

        [TestMethod]
        public async Task PostStock()
        {
            var result = await serviceClient.StockPOSTAsync(new Dictionary<string, int> {
                { "20", 5 },
                { "1", 100 },
            }).ConfigureAwait(false);

            Assert.IsTrue(result.Count > 0);
            Assert.IsTrue(result["20"] >= 5);
            Assert.IsTrue(result["1"] >= 100);
        }

        [TestMethod]
        public async Task PostBadStock()
        {
            await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await serviceClient.StockPOSTAsync(new Dictionary<string, int> {
                    { "xxx20", 5 },
                    { "1", 100 }
                }).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task PostOverflowStock()
        {
            await Assert.ThrowsExceptionAsync<ApiException>(async () =>
                await serviceClient.StockPOSTAsync(new Dictionary<string, int> {
                    { "4300000000", 1 },
                    { "1", 100 }
                }).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task CheckoutStock()
        {
            var result = await serviceClient.StockPOSTAsync(new Dictionary<string, int> {
                { "20", 5 },
                { "10", 4 },
                { "1", 100 },
            }).ConfigureAwait(false);

            Assert.IsTrue(result.Count > 0);
            Assert.IsTrue(result["20"] >= 5);
            Assert.IsTrue(result["10"] >= 4);
            Assert.IsTrue(result["1"] >= 100);

            var checkoutResult = await serviceClient.CheckoutAsync(new CheckoutRequest {
                Inserted = new Dictionary<string, int> {
                    { "50", 6}
                },
                Price = 227
            }).ConfigureAwait(false);

            Assert.AreEqual(1, checkoutResult["50"]);
            Assert.AreEqual(1, checkoutResult["20"]);
            Assert.AreEqual(3, checkoutResult["1"]);
        }
    }
}
