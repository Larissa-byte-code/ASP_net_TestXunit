using System.Net;
using System.Net.Http.Json;
using Xunit;
using SmarketApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SmarketApi.Tests
{
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<SmarketApi.Program>>
    {
        private readonly HttpClient _client;

        public AuthControllerTests(WebApplicationFactory<SmarketApi.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_CreatesUser()
        {
            var response = await _client.PostAsync("/api/auth/register?username=larissa&email=larissa@test.com&password=123456", null);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Utilisateur créé", content);
        }

        [Fact]
        public async Task Login_ReturnsToken()
        {
            // Assure-toi qu’un utilisateur existe déjà en base
            var response = await _client.PostAsync("/api/auth/login?email=larissa@test.com&password=123456", null);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
           var result = await response.Content.ReadFromJsonAsync<dynamic>();
                Assert.NotNull(result);
                Assert.NotNull(result?.token);

            
        }
    }
}
