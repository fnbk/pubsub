
namespace Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var topic = "ProductUpdate";
            Console.WriteLine($"Publisher started, sending messages on topic '{topic}', start typing a messae and press enter to send it");

            string input;
            do
            {
                input = Console.ReadLine() ?? string.Empty;
                if (!string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    PublishMessage(topic, input);
                    Console.WriteLine("Message sent: " + input);
                }
            } while (!string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase));

            Console.WriteLine("Publisher stopped.");
        }

        private static void PublishMessage(string topic, string input)
        {
            var publisher = new PubSub.Publisher();
            var obj = new Product { Title = input, Description = "demo description", Price = "demo price" };
            publisher.Publish(topic, obj);
        }
    }
}



