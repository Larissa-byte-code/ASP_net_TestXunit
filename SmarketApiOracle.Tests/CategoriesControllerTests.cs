using System.Net;
using System.Net.Http.Json;
using SmarketApiOracle.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SmarketApiOracle.Tests
{
    // Utilisation directe de WebApplicationFactory<Program>
    public class CategoriesControllerTests : IClassFixture<WebApplicationFactory<SmarketApiOracle.Program>>
    {
        private readonly HttpClient _client;

        public CategoriesControllerTests(WebApplicationFactory<SmarketApiOracle.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/categories/all");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var categories = await response.Content.ReadFromJsonAsync<List<TblCategory>>();
            Assert.NotNull(categories);
        }

        [Fact]
        public async Task Create_AddsCategory()
        {
            var newCategory = new TblCategory
            {
                CatIdvC="valeur fictif",
                CatName = "lar",
                CatDes  = "Description lar"
            };

            var response = await _client.PostAsJsonAsync("/api/categories/add", newCategory);

            // Log pour voir l’erreur si ça plante
            var contentString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"STATUS : {response.StatusCode}");
            Console.WriteLine($"CONTENT : {contentString}");

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<TblCategory>();
            Assert.NotNull(created);
            Assert.Equal("lar", created!.CatName);
            Assert.Equal("Description lar", created.CatDes);
            Assert.StartsWith("Cat-", created.CatIdvC);
        }

        [Fact]
        public async Task Update_ChangesCategory()
        {
            var newCategory = new TblCategory
            {
                CatIdvC="valeur fictif",
                CatName = "OldName",
                CatDes  = "Old description"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/categories/add", newCategory);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<TblCategory>();
            Assert.NotNull(created);

            created!.CatName = "UpdatedName";
            created.CatDes   = "Updated description";

            var updateResponse = await _client.PutAsJsonAsync($"/api/categories/update/{created.CatId}", created);
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

            var updated = await updateResponse.Content.ReadFromJsonAsync<TblCategory>();
            Assert.Equal("UpdatedName", updated!.CatName);
            Assert.Equal("Updated description", updated.CatDes);
        }

        [Fact]
        public async Task Delete_RemovesCategory()
        {
            var newCategory = new TblCategory
            {
                CatIdvC="valeur fictif",
                CatName = "DeleteMe",
                CatDes  = "To be deleted"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/categories/add", newCategory);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<TblCategory>();
            Assert.NotNull(created);

            var deleteResponse = await _client.DeleteAsync($"/api/categories/delete/{created!.CatId}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/categories/get/{created.CatId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
