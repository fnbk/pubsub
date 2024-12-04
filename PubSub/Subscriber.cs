using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO.Pipes;

namespace PubSub
{
    public class Subscriber : BackgroundService
    {
        private readonly List<SubscribeControllerMethod> _pubSubControllerMethods;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public Subscriber(
            List<SubscribeControllerMethod> pubSubControllerMethods,
            IServiceScopeFactory serviceScopeFactory
        )
        {
            _pubSubControllerMethods = pubSubControllerMethods;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Wait until the application is fully started before subscribing to topics.  
            await Task.Yield();

            // Subscribe to topics.  
            var subscriptionTasks = _pubSubControllerMethods.Select(method =>
            {
                return ListenToTopic(method.Topic, stoppingToken);
            }).ToList();

            // Wait for all subscriptions to complete or until cancellation is requested.  
            await Task.WhenAll(subscriptionTasks);
        }

        private async Task ListenToTopic(string topic, CancellationToken stoppingToken)
        {
            Console.WriteLine($"Listening to topic: {topic}");

            while (!stoppingToken.IsCancellationRequested) // Continues to try listening even if pipe is broken/disconnected  
            {
                using (var pipeClient = new NamedPipeClientStream(".", topic, PipeDirection.In, PipeOptions.Asynchronous))
                {
                    await pipeClient.ConnectAsync(stoppingToken);

                    var buffer = new byte[256];
                    using (var ms = new MemoryStream())
                    {
                        int readBytes;
                        while ((readBytes = await pipeClient.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, readBytes);
                        }
                        byte[] message = ms.ToArray();

                        // Invoke the message received callback 
                        InvokeControllerMethod(topic, message);
                    }
                }
            }
        }

        private void InvokeControllerMethod(string topic, byte[] message)
        {
            // find appropriate controller
            var controllerMethod = _pubSubControllerMethods.Find(e => e.Topic == topic);
            if (controllerMethod == null)
            {
                return;
            }

            // serialize byte[] into object
            string jsonString = System.Text.Encoding.UTF8.GetString(message);
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString, controllerMethod.ParameterType);
            if (obj == null)
            {
                return;
            }

            // create method instance
            var scope = _serviceScopeFactory.CreateAsyncScope();
            object classInstance = ActivatorUtilities.GetServiceOrCreateInstance(scope.ServiceProvider, controllerMethod.HandlerClass);
            var parameterArray = new object[] { obj };

            // invoke method
            #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.  
            Task<object> response = (Task<object>)controllerMethod.Handler.Invoke(classInstance, parameterArray);
            #pragma warning restore CS8600 
        }
    }
}
