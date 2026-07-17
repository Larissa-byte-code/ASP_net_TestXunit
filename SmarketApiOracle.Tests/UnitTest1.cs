using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SmarketApiOracle.Tests
{
    public class UnitTest1 : IClassFixture<WebApplicationFactory<SmarketApiOracle.Program>>
    {
        private readonly WebApplicationFactory<SmarketApiOracle.Program> _factory;

        public UnitTest1(WebApplicationFactory<SmarketApiOracle.Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetCategories_ReturnsOk()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/categories");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
