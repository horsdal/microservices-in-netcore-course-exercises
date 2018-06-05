using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;

namespace ShoppingCart
{
    public class Cart : NancyModule
    {
        private static Dictionary<int, IEnumerable<Product>> carts = new Dictionary<int, IEnumerable<Product>>();

        public Cart() : base("/shoppingcart")
        {
            Get("/{userId:int}", parameters =>
            {
                int userId = parameters.userId;
                if (!carts.ContainsKey(userId))
                    return HttpStatusCode.NotFound;
                return carts[userId];
            });

            Post("/{userId:int}", parameters =>
            {
                int userId = parameters.userId;
                if (!carts.ContainsKey(userId))
                    carts[userId] = Enumerable.Empty<Product>();
                IEnumerable<Product> products = this.Bind();
                carts[userId] = carts[userId].Concat(products);
                return carts[userId];
            });

            Delete("/{userid}", parameters => 
            {
                int userId = parameters.userId;
                if (carts.ContainsKey(userId))
                    carts.Remove(userId);
                return HttpStatusCode.NoContent;
            });
        }
    }
    
    public class Product
    {
        public string Name { get; set;}
    }
}