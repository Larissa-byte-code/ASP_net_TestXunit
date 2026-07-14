using System.Net;
using System.Net.Http.Json;
using SmarketApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SmarketApi.Tests
{
    // WebApplicationFactory démarre ton API en mémoire pour les tests
    public class ClientsControllerTests : IClassFixture<WebApplicationFactory<SmarketApi.Program>>
    {
        private readonly HttpClient _client;

        public ClientsControllerTests(WebApplicationFactory<SmarketApi.Program> factory)
        {
            _client = factory.CreateClient();
        }

        // Test GET /api/clients/all
        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/clients/all");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var clients = await response.Content.ReadFromJsonAsync<List<TblClient>>();
            Assert.NotNull(clients);
        }

        // Test POST /api/clients/add
        [Fact]
        public async Task Create_AddsClient()
        {
            var newClient = new TblClient
            {
                ClientCode  = "valeur fictive",
                ClientName = "TestClient",
                ClientPhone = "+261330000000",
                ClientEmail = "testclient@gmail.com",
                ClientAddress = "Tana 101",
                IsActive = true
            };

            var response = await _client.PostAsJsonAsync("/api/clients/add", newClient);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<TblClient>();
            Assert.NotNull(created);

            Assert.Equal("TestClient", created!.ClientName);
            Assert.Equal("testclient@gmail.com", created.ClientEmail);
        }

        // Test PUT /api/clients/update/{id}
        [Fact]
        public async Task Update_ChangesClient()
        {
            // Arrange : créer un client
            var newClient = new TblClient
            {
                ClientCode  = "valeur fictive",
                ClientName = "OldName",
                ClientPhone = "+261331111111",
                ClientEmail = "old@mail.com",
                ClientAddress = "Old address",
                IsActive = true
            };

            var createResponse = await _client.PostAsJsonAsync("/api/clients/add", newClient);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<TblClient>();
            Assert.NotNull(created);

            // Act : modifier le client
            created!.ClientName = "UpdatedName";
            created.ClientEmail = "updated@mail.com";

            var updateResponse = await _client.PutAsJsonAsync($"/api/clients/update/{created.ClientId}", created);

            // Assert
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            var updated = await updateResponse.Content.ReadFromJsonAsync<TblClient>();
            Assert.Equal("UpdatedName", updated!.ClientName);
            Assert.Equal("updated@mail.com", updated.ClientEmail);
        }

        // Test DELETE /api/clients/delete/{id}
        [Fact]
        public async Task Delete_RemovesClient()
        {
            // Arrange : créer un client
            var newClient = new TblClient
            {
                ClientCode  = "valeur fictive",
                ClientName = "DeleteMe",
                ClientPhone = "+261332222222",
                ClientEmail = "deleteme@mail.com",
                ClientAddress = "To be deleted",
                IsActive = true
            };

            var createResponse = await _client.PostAsJsonAsync("/api/clients/add", newClient);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<TblClient>();
            Assert.NotNull(created);

            // Act : supprimer le client
            var deleteResponse = await _client.DeleteAsync($"/api/clients/delete/{created!.ClientId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Vérifie que GET ne le retrouve plus
            var getResponse = await _client.GetAsync($"/api/clients/get/{created.ClientId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
