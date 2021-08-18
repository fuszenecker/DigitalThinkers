using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace tests
{
    [TestClass]
    public class IntegrationTests
    {
        const string serviceBaseUrl = "http://api:5000";
        const string HealthCheckEndpointPath = "/health";
        const string MetricsEndpointPath = "/metrics";
        const string StockEndpointPath = "/api/v1/Stock";
        const string CheckoutEndpointPath = "/api/v1/Checkout";

        const string CorrelationIdHeaderName = "X-Correlation-ID";

        private HttpClient httpClient;

        [TestInitialize]
        public void Initialize()
        {
            httpClient = new HttpClient() {
                BaseAddress = new Uri(serviceBaseUrl)
            };
        }

        [TestMethod]
        // [Ignore]
        public async Task HealthCheckWorks()
        {
            var result = await httpClient.GetAsync(HealthCheckEndpointPath).ConfigureAwait(false);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual("Healthy", await result.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        [TestMethod]
        // [Ignore]
        public async Task MetricsWorks()
        {
            var result = await httpClient.GetAsync(MetricsEndpointPath).ConfigureAwait(false);
            var responseBody = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, result.StatusCode);

            // Response body contains at least 20 lines:
            Assert.IsTrue(responseBody.Split('\n').Length > 20);
        }

        [TestMethod]
        // [Ignore]
        public async Task CorrelationIdWorks()
        {
            var correlationId = Guid.NewGuid().ToString();

            httpClient.DefaultRequestHeaders.Add(CorrelationIdHeaderName, correlationId);

            var result = await httpClient.GetAsync(MetricsEndpointPath).ConfigureAwait(false);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(correlationId, result.Headers.GetValues("X-Correlation-ID").First());
        }

        [TestMethod]
        // [Ignore]
        public async Task GetStock()
        {
            var result = await httpClient.GetAsync(StockEndpointPath).ConfigureAwait(false);

            var stock = JsonSerializer.Deserialize<Dictionary<uint, uint>>(await result.Content.ReadAsStringAsync());

            Assert.AreEqual(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(0, stock.Count);
        }

        [TestMethod]
        // [Ignore]
        public async Task PostStock()
        {
            var json = "{ \"20\": 5, \"1\": 100}";
            var data = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var result = await httpClient.PostAsync(StockEndpointPath, data).ConfigureAwait(false);

            var stock = JsonSerializer.Deserialize<Dictionary<uint, uint>>(
                await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            Assert.AreEqual(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.IsTrue(stock.Count > 0);
        }


        [TestMethod]
        // [Ignore]
        public async Task PostBadStock()
        {
            var json = "{ \"xxx\": 5, \"1\": 100}";
            var data = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var result = await httpClient.PostAsync(StockEndpointPath, data).ConfigureAwait(false);

            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        // [Ignore]
        public async Task CheckoutStock()
        {
            var json = "{ \"20\": 5, \"1\": 100}";
            var data = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var result = await httpClient.PostAsync(StockEndpointPath, data).ConfigureAwait(false);

            var stock = JsonSerializer.Deserialize<Dictionary<uint, uint>>(
                await result.Content.ReadAsStringAsync().ConfigureAwait(false));

            Assert.AreEqual(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.IsTrue(stock.Count > 0);

            var checkoutJson = "{ \"inserted\": {\"20\": 3, \"1\": 100}, \"price\": 50 }";
            var checkoutData = new StringContent(checkoutJson, System.Text.Encoding.UTF8, "application/json");

            var checkoutResult = await httpClient.PostAsync(CheckoutEndpointPath, checkoutData).ConfigureAwait(false);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, checkoutResult.StatusCode);

            var payBack = JsonSerializer.Deserialize<Dictionary<uint, uint>>(
                await checkoutResult.Content.ReadAsStringAsync().ConfigureAwait(false));

            Assert.IsTrue(payBack.Count > 0);
        }
    }
}
