using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Api;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    internal class e2eTests
    {
        private HttpClient _apiClient;
        private TestServer _testApi;

        [SetUp]
        public void SetUp()
        {
            _testApi = new TestServer(new WebHostBuilder().ConfigureServices(services => services.AddAutofac())
                .UseStartup<Startup>());

            _apiClient = _testApi.CreateClient();
        }

        #region Post
        [Test]
        public async Task BookingSuccessfulResponse()
        {
            var response = await _apiClient.PostAsync("api/book", new StringContent(JsonConvert.SerializeObject(new Booking
            {
                Id = "book022",
                NumberOfBeds = 1,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2), 
            }), Encoding.UTF8, "application/json"));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var resultJson = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<BookingResponse>(resultJson); 
            Assert.That(result.IsError, Is.False);
        }

        [Test]
        public async Task BookingFailureResponse()
        {
            var response = await _apiClient.PostAsync("api/book", new StringContent(JsonConvert.SerializeObject(new Booking
            {
                Id = "book022",
                NumberOfBeds = 2,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1), 
            }), Encoding.UTF8, "application/json"));

            Assert.That(response.StatusCode == HttpStatusCode.OK);

            var resultJson = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<BookingResponse>(resultJson); 
            Assert.That(result.IsError, Is.True);
        }

        [Test]
        public async Task BookingBackBadRequest()
        {
            var response = await _apiClient.PostAsync("api/book", new StringContent(JsonConvert.SerializeObject(new Booking
            {
                Id = "book022",
                NumberOfBeds = 0,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow,
            }), Encoding.UTF8, "application/json"));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
        #endregion

        #region Put
        [Test]
        public async Task BookingSuccessfulUpdateResponse()
        {
            var response = await _apiClient.PostAsync("api/book", new StringContent(JsonConvert.SerializeObject(new Booking
            {
                Id = "book023",
                NumberOfBeds = 1,
                StartDate = DateTime.UtcNow.AddDays(4),
                EndDate = DateTime.UtcNow.AddDays(5),
            }), Encoding.UTF8, "application/json"));

            response = await _apiClient.PutAsync("api/book", new StringContent(JsonConvert.SerializeObject(new Booking
            {
                Id = "book023",
                NumberOfBeds = 1,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3),
            }), Encoding.UTF8, "application/json"));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var resultJson = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<BookingResponse>(resultJson);
            Assert.That(result.IsError, Is.False);
        }

        [Test]
        public async Task BookingFailureUpdateResponse()
        {
            var response = await _apiClient.PutAsync("api/book", new StringContent(JsonConvert.SerializeObject(new Booking
            {
                Id = "book033",
                NumberOfBeds = 2,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
            }), Encoding.UTF8, "application/json"));

            Assert.That(response.StatusCode == HttpStatusCode.OK);

            var resultJson = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<BookingResponse>(resultJson);
            Assert.That(result.IsError, Is.True);
        }
        #endregion

        #region Get
        [Test]
        public async Task BookingFailureGetBookIDResponse()
        {
            var response = await _apiClient.GetAsync("api/book?id=book024");

            var resultJson = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<BookingResponse>(resultJson);
            Assert.That(result.IsError, Is.True);
        }

        [Test]
        public async Task BookingSuccessGetAllBookResponse()
        {
            var response = await _apiClient.PostAsync("api/book", new StringContent(JsonConvert.SerializeObject(new Booking
            {
                Id = "book025",
                NumberOfBeds = 1,
                StartDate = DateTime.UtcNow.AddDays(11),
                EndDate = DateTime.UtcNow.AddDays(12),
            }), Encoding.UTF8, "application/json"));

            Assert.That(response.StatusCode == HttpStatusCode.OK);

            response = await _apiClient.GetAsync("api/book/all");

            var resultJson = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<BookingResponse>(resultJson);
            Assert.That(result.IsError, Is.False);
        }

        [Test]
        public async Task BookingSuccessGetBookIDResponse()
        {
            var response = await _apiClient.PostAsync("api/book", new StringContent(JsonConvert.SerializeObject(new Booking
            {
                Id = "book026",
                NumberOfBeds = 1,
                StartDate = DateTime.UtcNow.AddDays(14),
                EndDate = DateTime.UtcNow.AddDays(15),
            }), Encoding.UTF8, "application/json"));

            Assert.That(response.StatusCode == HttpStatusCode.OK);

            response = await _apiClient.GetAsync("api/book?id=book026");

            var resultJson = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<BookingResponse>(resultJson);
            Assert.That(result.IsError, Is.False);
        }
        #endregion
    }
}