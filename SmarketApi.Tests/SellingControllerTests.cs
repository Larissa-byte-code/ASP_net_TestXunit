using System.Net;
using System.Net.Http.Json;
using SmarketApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SmarketApi.Tests
{
    public class SellingControllerTests : IClassFixture<WebApplicationFactory<SmarketApi.Program>>
    {
        private readonly HttpClient _client;

        public SellingControllerTests(WebApplicationFactory<SmarketApi.Program> factory)
        {
            _client = factory.CreateClient();
        }

        // Test GET /api/selling/all
        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/selling/all");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var ventes = await response.Content.ReadFromJsonAsync<List<TblSelling>>();
            Assert.NotNull(ventes);

            foreach (var v in ventes!)
            {
                Console.WriteLine($"Vente: {v.VenteId} | {v.Numfacture} | {v.SellerName} | {v.ClientName} | {v.TotalAmount}");
            }
        }

        // Test POST /api/selling/add
        [Fact]
        public async Task Create_AddsSelling()
        {
            var newSelling = new VenteIndexViewModel
            {
                Selling = new TblSelling
                {
                    Numfacture     ="valeur fictif",
                    SellerName     = "TestSeller",
                    ClientName     = "TestClient",
                    ModeDePaiement = "Cash",
                    DateVente      = DateTime.Now
                },
                Details = new List<TblDetailSelling>
                {
                    new TblDetailSelling { ProductId = 1, ProductName = "coca cola", Qty = 2, Price = 1000 },
                    new TblDetailSelling { ProductId = 4, ProductName = "yaourt nature", Qty = 1, Price = 1500 }
                }
            };

            var response = await _client.PostAsJsonAsync("/api/selling/add", newSelling);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<TblSelling>();
            Assert.NotNull(created);

            Console.WriteLine($"Vente créée: {created!.VenteId} | {created.Numfacture} | Total: {created.TotalAmount}");
        }

        // Test GET /api/selling/get/{id}
        [Fact]
        public async Task GetById_ReturnsSellingWithDetails()
        //GetById
        {
            // Arrange : créer une vente
            var newSelling = new VenteIndexViewModel
            {
                Selling = new TblSelling
                {
                    Numfacture     ="valeur fictif",
                    SellerName     = "Jean Rakoto",
                    ClientName     = "Jean Paul",
                    ModeDePaiement = "Carte bancaire",
                    DateVente      = DateTime.Now
                },
                Details = new List<TblDetailSelling>
                {
                    new TblDetailSelling { ProductId = 2, ProductName = "Jus d'orange", Qty = 3, Price = 2000 }
                }
            };

            var createResponse = await _client.PostAsJsonAsync("/api/selling/add", newSelling);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<TblSelling>();
            Assert.NotNull(created);

            // Act : récupérer la vente
            var getResponse = await _client.GetAsync($"/api/selling/get/{created!.VenteId}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var venteWithDetails = await getResponse.Content.ReadFromJsonAsync<object>();
            Assert.NotNull(venteWithDetails);

            Console.WriteLine($"Vente récupérée: {created.VenteId} | {created.Numfacture}");
        }

        // Test DELETE /api/selling/delete/{id}
        [Fact]
        public async Task Delete_RemovesSelling()
        {
            // Arrange : créer une vente
            var newSelling = new VenteIndexViewModel
            {
                Selling = new TblSelling
                {
                    Numfacture     ="valeur fictif",
                    SellerName     = "DeleteSeller",
                    ClientName     = "DeleteClient",
                    ModeDePaiement = "Mobile Money",
                    DateVente      = DateTime.Now
                },
                Details = new List<TblDetailSelling>
                {
                    new TblDetailSelling { ProductId = 3, ProductName = "Lait entier", Qty = 2, Price = 1000 }
                }
            };

            var createResponse = await _client.PostAsJsonAsync("/api/selling/add", newSelling);
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<TblSelling>();
            Assert.NotNull(created);

            // Act : supprimer la vente
            var deleteResponse = await _client.DeleteAsync($"/api/selling/delete/{created!.VenteId}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Vérifie que GET ne la retrouve plus
            var getResponse = await _client.GetAsync($"/api/selling/get/{created.VenteId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

            Console.WriteLine($"Vente supprimée: {created.VenteId} | {created.Numfacture}");
        }
    }
}
