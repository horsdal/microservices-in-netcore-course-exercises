using System.Collections.Generic;
using System.Linq;
using Marten;
using Nancy;
using Nancy.ModelBinding;

namespace ShoppingCart
{
    public class Cart : NancyModule
    {

        public Cart(IDocumentStore documentStore) : base("/shoppingcart")
        {
            Get("/{userId:int}", async parameters =>
            {
                int userId = parameters.userId;
                using (var session = documentStore.LightweightSession())
                {
                    var cart = await session.LoadAsync<ShoppingCart>(userId);
                    return cart ?? (object) HttpStatusCode.NotFound;
                }
            });

            Post("/{userId:int}", async parameters =>
            {
                int userId = parameters.userId;
                IEnumerable<Product> products = this.Bind();
                using (var session = documentStore.LightweightSession())
                {
                    var cart = await session.LoadAsync<ShoppingCart>(userId) ?? new ShoppingCart { Id = userId, Products = new List<Product>()};
                    cart.Products.AddRange(products);
                    session.Store(cart);
                    await session.SaveChangesAsync();
                    return cart;
                }
            });

            Delete("/{userid}", async parameters => 
            {
                using (var session = documentStore.LightweightSession())
                {
                    session.Delete<ShoppingCart>((int) parameters.userId);
                    await session.SaveChangesAsync();
                }

                return HttpStatusCode.NoContent;
            });
        }
    }
    
    public class Product
    {
        public string Name { get; set;}
    }

    public class ShoppingCart
    {
        public int Id;
        public List<Product> Products { get; set; }
    }
}