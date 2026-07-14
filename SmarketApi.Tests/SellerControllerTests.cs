using System.Net;
using System.Net.Http.Json;
using SmarketApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SmarketApi.Tests
{
    public class SellersControllerTests : IClassFixture<WebApplicationFactory<SmarketApi.Program>>
    {
        private readonly HttpClient _client;

        public SellersControllerTests(WebApplicationFactory<SmarketApi.Program> factory)
        {
            _client = factory.CreateClient();
        }

        // Test GET /api/sellers/all
        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/sellers/all");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var sellers = await response.Content.ReadFromJsonAsync<List<TblSeller>>();
            Assert.NotNull(sellers);

            foreach (var s in sellers!)
            {
                Console.WriteLine($"Seller: {s.SellerId} | {s.SellerName} | {s.SellerPhone} | {s.Role}");
            }
        }

        // Test POST /api/sellers/add
        [Fact]
        public async Task Create_AddsSeller()
        {
            var newSeller = new TblSeller
            {
                SellerIdvC="valeur fictif",
                SellerName  = "TestSeller",
                SellerAge   = 28,
                SellerPhone = "330000000",
                SellerPass  = "pass123",
                Role        = "User"
            };

            var response = await _client.PostAsJsonAsync("/api/sellers/add", newSeller);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<TblSeller>();
            Assert.NotNull(created);

            Assert.Equal("TestSeller", created!.SellerName);
            Assert.Equal("User", created.Role);

            Console.WriteLine($"Seller créé: {created.SellerId} | {created.SellerName} | {created.SellerPhone} | {created.Role}");
        }

        // Test PUT /api/sellers/update/{id}
        [Fact]
        public async Task Update_ChangesSeller()
        {
            // Arrange : créer un vendeur
            var newSeller = new TblSeller
            {
                SellerIdvC="valeur fictif",
                SellerName  = "OldName",
                SellerAge   = 30,
                SellerPhone = "331111111",
                SellerPass  = "oldpass",
                Role        = "User"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/sellers/add", newSeller);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<TblSeller>();
            Assert.NotNull(created);

            // Act : modifier le vendeur
            created!.SellerName = "UpdatedName";
            created.SellerAge   = 35;
            created.SellerPass  = "newpass";
            created.Role        = "Admin";

            var updateResponse = await _client.PutAsJsonAsync($"/api/sellers/update/{created.SellerId}", created);
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

            var updated = await updateResponse.Content.ReadFromJsonAsync<TblSeller>();
            Assert.Equal("UpdatedName", updated!.SellerName);
            Assert.Equal("Admin", updated.Role);

            Console.WriteLine($"Seller mis à jour: {updated.SellerId} | {updated.SellerName} | {updated.SellerPhone} | {updated.Role}");
        }

        // Test DELETE /api/sellers/delete/{id}
        [Fact]
        public async Task Delete_RemovesSeller()
        {
            // Arrange : créer un vendeur
            var newSeller = new TblSeller
            {
                SellerIdvC="valeur fictif",
                SellerName  = "DeleteMe",
                SellerAge   = 40,
                SellerPhone = "332222222",
                SellerPass  = "deletepass",
                Role        = "User"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/sellers/add", newSeller);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<TblSeller>();
            Assert.NotNull(created);

            // Act : supprimer le vendeur
            var deleteResponse = await _client.DeleteAsync($"/api/sellers/delete/{created!.SellerId}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Vérifie que GET ne le retrouve plus
            var getResponse = await _client.GetAsync($"/api/sellers/get/{created.SellerId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

            Console.WriteLine($"Seller supprimé: {created.SellerId} | {created.SellerName}");
        }
    }
}
