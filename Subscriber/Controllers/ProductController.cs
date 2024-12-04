using PubSub;
using System.Text.Json;

namespace Subscriber.Controllers
{
    public class ProductController : SubscribeController
    {
        public ProductController()
        {
        }

        [Topic("ProductUpdate")]
        public async Task<object> ProductUpdate(Product product)
        {
            Console.WriteLine($"ProductUpdate message received:");
            Console.WriteLine(JsonSerializer.Serialize(product));

            return await Task.FromResult(new { X = "YZ" });
        }
    }

}
