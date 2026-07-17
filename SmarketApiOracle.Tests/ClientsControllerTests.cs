using System.Net;
using System.Net.Http.Json;
using SmarketApiOracle.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SmarketApiOracle.Tests
{
    // Tests pour ClientsController
    public class ClientsControllerTests : IClassFixture<WebApplicationFactory<SmarketApiOracle.Program>>
    {
        private readonly HttpClient _client;

        public ClientsControllerTests(WebApplicationFactory<SmarketApiOracle.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/clients/all");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var clients = await response.Content.ReadFromJsonAsync<List<TblClient>>();
            Assert.NotNull(clients);
        }

        [Fact]
        public async Task Create_AddsClient()
        {
            var newClient = new TblClient
            {
                ClientCode   = "PENDING",
                ClientName    = "Larissa",
                ClientPhone   = "0320000000",
                ClientEmail   = "larissa@test.com",
                ClientAddress = "Antananarivo",
                IsActive      = true
            };

            var response = await _client.PostAsJsonAsync("/api/clients/add", newClient);

            var contentString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"STATUS : {response.StatusCode}");
            Console.WriteLine($"CONTENT : {contentString}");

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<TblClient>();
            Assert.NotNull(created);
            Assert.Equal("Larissa", created!.ClientName);
            Assert.Equal("0320000000", created.ClientPhone);
            Assert.StartsWith("CL-", created.ClientCode); // code formaté
        }

        [Fact]
        public async Task Update_ChangesClient()
        {
            var newClient = new TblClient
            {
                ClientCode   = "PENDING",
                ClientName    = "OldName",
                ClientPhone   = "0321111111",
                ClientEmail   = "old@test.com",
                ClientAddress = "Old address",
                IsActive      = true
            };

            var createResponse = await _client.PostAsJsonAsync("/api/clients/add", newClient);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<TblClient>();
            Assert.NotNull(created);

            created!.ClientName    = "UpdatedName";
            created.ClientPhone    = "0322222222";
            created.ClientEmail    = "updated@test.com";
            created.ClientAddress  = "Updated address";
            created.IsActive       = false;

            var updateResponse = await _client.PutAsJsonAsync($"/api/clients/update/{created.ClientId}", created);
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

            var updated = await updateResponse.Content.ReadFromJsonAsync<TblClient>();
            Assert.Equal("UpdatedName", updated!.ClientName);
            Assert.Equal("0322222222", updated.ClientPhone);
            Assert.Equal("updated@test.com", updated.ClientEmail);
            Assert.Equal("Updated address", updated.ClientAddress);
            Assert.False(updated.IsActive);
        }

        [Fact]
        public async Task Delete_RemovesClient()
        {
            var newClient = new TblClient
            {
                ClientCode   = "PENDING",
                ClientName    = "DeleteMe",
                ClientPhone   = "0323333333",
                ClientEmail   = "delete@test.com",
                ClientAddress = "To be deleted",
                IsActive      = true
            };

            var createResponse = await _client.PostAsJsonAsync("/api/clients/add", newClient);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<TblClient>();
            Assert.NotNull(created);

            var deleteResponse = await _client.DeleteAsync($"/api/clients/delete/{created!.ClientId}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/clients/get/{created.ClientId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
