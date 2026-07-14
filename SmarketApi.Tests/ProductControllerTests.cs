using System.Net;
using System.Net.Http.Json;
using SmarketApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SmarketApi.Tests
{
    public class ProductsControllerTests : IClassFixture<WebApplicationFactory<SmarketApi.Program>>
    {
        private readonly HttpClient _client;

        public ProductsControllerTests(WebApplicationFactory<SmarketApi.Program> factory)
        {
            _client = factory.CreateClient();
        }

        // Test GET /api/products/all
        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/products/all");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var products = await response.Content.ReadFromJsonAsync<List<TblProduct>>();
            Assert.NotNull(products);

            // Affiche les produits dans la console
            foreach (var p in products!)
            {
                Console.WriteLine($"Produit: {p.prdId} | {p.prdName} | {p.prdCat} | {p.prdIdvC}");
            }
        }

        // Test POST /api/products/add
        [Fact]
        public async Task Create_AddsProduct()
        {
            var newProduct = new TblProduct
            {
                prdIdvC="valeur fictif",
                prdName = "TestProduct",
                prdCat  = "TestCategory"
            };

            var response = await _client.PostAsJsonAsync("/api/products/add", newProduct);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<TblProduct>();
            Assert.NotNull(created);

            Assert.Equal("TestProduct", created!.prdName);
            Assert.Equal("TestCategory", created.prdCat);

            Console.WriteLine($"Produit créé: {created.prdId} | {created.prdName} | {created.prdCat} | {created.prdIdvC}");
        }

        // Test PUT /api/products/update/{id}
        [Fact]
        public async Task Update_ChangesProduct()
        {
            // Arrange : créer un produit
            var newProduct = new TblProduct
            {
                prdIdvC="valeur fictif",
                prdName = "OldName",
                prdCat  = "OldCategory"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/products/add", newProduct);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<TblProduct>();
            Assert.NotNull(created);

            // Act : modifier le produit
            created!.prdName = "UpdatedName";
            created.prdCat   = "UpdatedCategory";

            var updateResponse = await _client.PutAsJsonAsync($"/api/products/update/{created.prdId}", created);
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

            var updated = await updateResponse.Content.ReadFromJsonAsync<TblProduct>();
            Assert.Equal("UpdatedName", updated!.prdName);
            Assert.Equal("UpdatedCategory", updated.prdCat);

            Console.WriteLine($"Produit mis à jour: {updated.prdId} | {updated.prdName} | {updated.prdCat} | {updated.prdIdvC}");
        }

        // Test DELETE /api/products/delete/{id}
        [Fact]
        public async Task Delete_RemovesProduct()
        {
            // Arrange : créer un produit
            var newProduct = new TblProduct
            {
                prdIdvC="valeur fictif",
                prdName = "DeleteMe",
                prdCat  = "ToBeDeleted"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/products/add", newProduct);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<TblProduct>();
            Assert.NotNull(created);

            // Act : supprimer le produit
            var deleteResponse = await _client.DeleteAsync($"/api/products/delete/{created!.prdId}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Vérifie que GET ne le retrouve plus
            var getResponse = await _client.GetAsync($"/api/products/get/{created.prdId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

            Console.WriteLine($"Produit supprimé: {created.prdId} | {created.prdName}");
        }
    }
}
