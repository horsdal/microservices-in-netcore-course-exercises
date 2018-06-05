using System;
using System.Linq;
using System.Threading.Tasks;
using Nancy;
using Nancy.Testing;
using ShoppingCart;
using Xunit;

namespace ShoppingCartTests
{
    public class ShoppingCart_Should
    {
        private readonly Browser sut; 
        private readonly int userId;

        public ShoppingCart_Should()
        {
            userId = 123;
            sut = new Browser(with => with.Module<Cart>(), defaults => defaults.Accept("application/json"));
        }

        [Fact]
        public async Task allow_for_adding_items()
        {
            await AddProductsToCart("xbox", "playstation");

            var actual = await sut.Get($"/shoppingcart/{userId}");
            var actualContent = actual.Body.DeserializeJson<Product[]>();

            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
            Assert.Equal(2, actualContent.Length);
            Assert.Contains("xbox", actualContent.Select(x => x.Name));
            Assert.Contains("playstation", actualContent.Select(x => x.Name));
        }

        [Fact]
        public async Task allow_deleting_cart()
        {
            await AddProductsToCart("pencil");

            await sut.Delete($"/shoppingcart/{userId}");
            var actual = await sut.Get($"/shoppingcart/{userId}");

            Assert.Equal(HttpStatusCode.NotFound, actual.StatusCode);
        }

        private async Task AddProductsToCart(params string[] productNames)
        {
            await sut.Post(
                $"/shoppingcart/{userId}", 
                with => with.JsonBody(productNames.Select(x => new { Name = x })));
        }

    }
}
