using System.Net;
using System.Net.Http.Json;
using SmarketApiOracle.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SmarketApiOracle.Tests
{
    public class SellingControllerTests : IClassFixture<WebApplicationFactory<SmarketApiOracle.Program>>
    {
        private readonly HttpClient _client;

        public SellingControllerTests(WebApplicationFactory<SmarketApiOracle.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/selling/all");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var ventes = await response.Content.ReadFromJsonAsync<List<TblSelling>>();
            Assert.NotNull(ventes);
        }

        [Fact]
        public async Task Create_AddsSelling()
        {
            var model = new VenteIndexViewModel
            {
                Selling = new TblSelling
                {
                    SellerName    = "Larissa",
                    ClientName    = "ClientTest",
                    ModeDePaiement= "Cash",
                    Numfacture    = "PENDING"
                },
                Details = new List<TblDetailSelling>
                {
                    new TblDetailSelling
                    {
                        ProductId   = "1",   
                        ProductName = "Laptop",
                        Qty         = 2,
                        Price       = 500m
                    }
                }
            };

            var response = await _client.PostAsJsonAsync("/api/selling/add", model);

            var contentString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"STATUS : {response.StatusCode}");
            Console.WriteLine($"CONTENT : {contentString}");

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<TblSelling>();
            Assert.NotNull(created);
            Assert.StartsWith("FACTURE-", created!.Numfacture);
        }

        [Fact]
        public async Task Delete_RemovesSelling()
        {
            var model = new VenteIndexViewModel
            {
                Selling = new TblSelling
                {
                    SellerName    = "DeleteSeller",
                    ClientName    = "DeleteClient",
                    ModeDePaiement= "Cash",
                    Numfacture    = "PENDING"
                },
                Details = new List<TblDetailSelling>
                {
                    new TblDetailSelling
                    {
                        ProductId   = "1",   
                        ProductName = "Phone",
                        Qty         = 1,
                        Price       = 200m
                    }
                }
            };

            var createResponse = await _client.PostAsJsonAsync("/api/selling/add", model);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<TblSelling>();
            Assert.NotNull(created);

            var deleteResponse = await _client.DeleteAsync($"/api/selling/delete/{created!.VenteId}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/selling/get/{created.VenteId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
