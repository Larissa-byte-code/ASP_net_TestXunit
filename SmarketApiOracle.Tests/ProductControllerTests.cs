using System.Net;
using System.Net.Http.Json;
using SmarketApiOracle.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SmarketApiOracle.Tests
{
    // Tests pour ProductsController
    public class ProductControllerTests : IClassFixture<WebApplicationFactory<SmarketApiOracle.Program>>
    {
        private readonly HttpClient _client;

        public ProductControllerTests(WebApplicationFactory<SmarketApiOracle.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/products/all");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var products = await response.Content.ReadFromJsonAsync<List<TblProduct>>();
            Assert.NotNull(products);
        }

        [Fact]
        public async Task Create_AddsProduct()
        {
            var newProduct = new TblProduct
            {
                prdName = "Laptop",
                prdCat  = "Electronics",
                prdIdvC = "PENDING" // valeur par défaut
            };

            var response = await _client.PostAsJsonAsync("/api/products/add", newProduct);

            var contentString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"STATUS : {response.StatusCode}");
            Console.WriteLine($"CONTENT : {contentString}");

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<TblProduct>();
            Assert.NotNull(created);
            Assert.Equal("Laptop", created!.prdName);
            Assert.Equal("Electronics", created.prdCat);
            Assert.StartsWith("PR-", created.prdIdvC); // code formaté
        }

        [Fact]
        public async Task Update_ChangesProduct()
        {
            var newProduct = new TblProduct
            {
                prdName = "OldProduct",
                prdCat  = "OldCategory",
                prdIdvC = "PENDING"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/products/add", newProduct);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<TblProduct>();
            Assert.NotNull(created);

            created!.prdName = "UpdatedProduct";
            created.prdCat   = "UpdatedCategory";

            var updateResponse = await _client.PutAsJsonAsync($"/api/products/update/{created.prdId}", created);
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

            var updated = await updateResponse.Content.ReadFromJsonAsync<TblProduct>();
            Assert.Equal("UpdatedProduct", updated!.prdName);
            Assert.Equal("UpdatedCategory", updated.prdCat);
        }

        [Fact]
        public async Task Delete_RemovesProduct()
        {
            var newProduct = new TblProduct
            {
                prdName = "DeleteMe",
                prdCat  = "ToDelete",
                prdIdvC = "PENDING"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/products/add", newProduct);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<TblProduct>();
            Assert.NotNull(created);

            var deleteResponse = await _client.DeleteAsync($"/api/products/delete/{created!.prdId}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/products/get/{created.prdId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
