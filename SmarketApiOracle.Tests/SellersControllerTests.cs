using System.Net;
using System.Net.Http.Json;
using SmarketApiOracle.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SmarketApiOracle.Tests
{
    // Tests pour SellersController
    public class SellersControllerTests : IClassFixture<WebApplicationFactory<SmarketApiOracle.Program>>
    {
        private readonly HttpClient _client;

        public SellersControllerTests(WebApplicationFactory<SmarketApiOracle.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/sellers/all");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var sellers = await response.Content.ReadFromJsonAsync<List<TblSeller>>();
            Assert.NotNull(sellers);
        }

        [Fact]
        public async Task Create_AddsSeller()
        {
            var newSeller = new TblSeller
            {
                SellerName  = "Larissa",
                SellerAge   = 30,
                SellerPhone = "0321234567",
                SellerPass  = "password123",
                Role        = "Admin",
                SellerIdvC  = "PENDING" // valeur par défaut
            };

            var response = await _client.PostAsJsonAsync("/api/sellers/add", newSeller);

            var contentString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"STATUS : {response.StatusCode}");
            Console.WriteLine($"CONTENT : {contentString}");

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<TblSeller>();
            Assert.NotNull(created);
            Assert.Equal("Larissa", created!.SellerName);
            Assert.Equal(30, created.SellerAge);
            Assert.StartsWith("Sel-", created.SellerIdvC); // code formaté
            Assert.StartsWith("+261", created.SellerPhone); // préfixe Madagascar
        }

        [Fact]
        public async Task Update_ChangesSeller()
        {
            var newSeller = new TblSeller
            {
                SellerName  = "OldName",
                SellerAge   = 25,
                SellerPhone = "0329876543",
                SellerPass  = "oldpass",
                Role        = "User",
                SellerIdvC  = "PENDING"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/sellers/add", newSeller);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<TblSeller>();
            Assert.NotNull(created);

            created!.SellerName  = "UpdatedName";
            created.SellerAge    = 26;
            created.SellerPhone  = "0331111111";
            created.SellerPass   = "newpass";
            created.Role         = "Manager";

            var updateResponse = await _client.PutAsJsonAsync($"/api/sellers/update/{created.SellerId}", created);
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

            var updated = await updateResponse.Content.ReadFromJsonAsync<TblSeller>();
            Assert.Equal("UpdatedName", updated!.SellerName);
            Assert.Equal(26, updated.SellerAge);
            Assert.Equal("newpass", updated.SellerPass);
            Assert.Equal("Manager", updated.Role);
            Assert.StartsWith("+261", updated.SellerPhone);
        }

        [Fact]
        public async Task Delete_RemovesSeller()
        {
            var newSeller = new TblSeller
            {
                SellerName  = "DeleteMe",
                SellerAge   = 40,
                SellerPhone = "0342222222",
                SellerPass  = "deletepass",
                Role        = "User",
                SellerIdvC  = "PENDING"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/sellers/add", newSeller);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<TblSeller>();
            Assert.NotNull(created);

            var deleteResponse = await _client.DeleteAsync($"/api/sellers/delete/{created!.SellerId}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/sellers/get/{created.SellerId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
