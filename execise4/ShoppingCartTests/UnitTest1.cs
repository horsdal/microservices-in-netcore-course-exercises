using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
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
            sut = new Browser(with => 
                {
                    with.Module<Cart>();
                    with.Dependency<IDocumentStore>(DocumentStore.For(x => 
                    {
                        x.Connection("host=localhost;database=shoppingcart;password=mysecretpassword;username=postgres");
                    }));
                },
                defaults => defaults.Accept("application/json"));
        }

        [Fact]
        public async Task allow_for_adding_items()
        {
            await sut.Delete($"/shoppingcart/{userId}");
            await AddProductsToCart("xbox", "playstation");

            var actual = await sut.Get($"/shoppingcart/{userId}");
            var actualContent = actual.Body.DeserializeJson<ShoppingCart.ShoppingCart>();

            Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
            Assert.Equal(2, actualContent.Products.Count);
            Assert.Contains("xbox", actualContent.Products.Select(x => x.Name));
            Assert.Contains("playstation", actualContent.Products.Select(x => x.Name));
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
