using System.Net;
using System.Net.Http.Json;
using SmarketApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SmarketApi.Tests
{
    // WebApplicationFactory démarre ton API en mémoire pour les tests
    public class CategoriesControllerTests : IClassFixture<WebApplicationFactory<SmarketApi.Program>>
    {
        private readonly HttpClient _client;

        public CategoriesControllerTests(WebApplicationFactory<SmarketApi.Program> factory)
        {
            _client = factory.CreateClient();
        }
        //Constructeur → initialise la classe de test.

        //WebApplicationFactory → démarre ton API en mémoire pour les tests.

        //CreateClient → fournit un HttpClient pour appeler ton API comme si elle tournait réellement.
        // Test GET /api/categories
        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/categories/all"); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var categories = await response.Content.ReadFromJsonAsync<List<TblCategory>>();
            Assert.NotNull(categories);
        }

        // Test POST /api/categories/add
        [Fact]
        public async Task Create_AddsCategory()
        {
            var newCategory = new TblCategory 
            { 
                CatIdvC = "valeur fictive",   
                CatName = "lar", 
                CatDes = "Description lar" 
            };

            var response = await _client.PostAsJsonAsync("/api/categories/add", newCategory);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<TblCategory>();
            Assert.NotNull(created);

            // Vérifie que l’API a bien enregistré et renvoyé les données
            Assert.Equal("lar", created!.CatName);
            Assert.Equal("Description lar", created.CatDes);

        }

        // Test PUT /api/categories/update/{id}
        [Fact]
        public async Task Update_ChangesCategory()
        {
            // Arrange : créer une catégorie
            var newCategory = new TblCategory
            {
                CatIdvC = "valeur fictive",
                CatName = "OldName",
                CatDes = "Old description"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/categories/add", newCategory);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<TblCategory>();
            Assert.NotNull(created);

            // Act : modifier la catégorie
            created!.CatName = "UpdatedName";
            created.CatDes = "Updated description";

            var updateResponse = await _client.PutAsJsonAsync($"/api/categories/update/{created.CatId}", created);

            // Assert
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            var updated = await updateResponse.Content.ReadFromJsonAsync<TblCategory>();
            Assert.Equal("UpdatedName", updated!.CatName);
            Assert.Equal("Updated description", updated.CatDes);
        }

        // Test DELETE /api/categories/delete/{id}
        [Fact]
        public async Task Delete_RemovesCategory()
        {
            // Arrange : créer une catégorie
            
            var newCategory = new TblCategory
            {
                CatIdvC = "valeur fictive",
                CatName = "DeleteMe",
                CatDes = "To be deleted"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/categories/add", newCategory);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<TblCategory>();
            Assert.NotNull(created);

            // Act : supprimer la catégorie
            var deleteResponse = await _client.DeleteAsync($"/api/categories/delete/{created!.CatId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Vérifie que GET ne la retrouve plus
            var getResponse = await _client.GetAsync($"/api/categories/get/{created.CatId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
