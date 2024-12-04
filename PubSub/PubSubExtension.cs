using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace PubSub
{
    //
    // PubSubExtension
    //

    public static class PubSubExtension
    {
        public static IServiceCollection AddPubSub(this IServiceCollection services, string applicationName)
        {
            var SubscribeControllerMethods = FindSubscribeControllerMethods(applicationName);

            services.AddSingleton(SubscribeControllerMethods);
            services.AddScoped<Publisher>();
            services.AddSingleton<Subscriber>();
            services.AddHostedService<Subscriber>(provider => provider.GetRequiredService<Subscriber>());

            return services;
        }

        private static List<SubscribeControllerMethod> FindSubscribeControllerMethods(string applicationName)
        {
            // Load the assembly containing the controllers using the application name
            var assembly = Assembly.Load(new AssemblyName(applicationName));

            // Find all types that are valid SubscribeControllers:
            // public, non-abstract classes, not including the SubscribeController class itself
            var subscribeControllerTypes = assembly.DefinedTypes
                .Where(typeInfo =>
                    typeInfo.IsPublic &&
                    typeInfo.IsClass &&
                    !typeInfo.IsAbstract &&
                    typeInfo != typeof(SubscribeController) &&
                    typeof(SubscribeController).IsAssignableFrom(typeInfo));

            // Find all methods in these types that have a TopicAttribute and map them to   
            // SubscribeControllerMethod instances
            var subscribeControllerMethods = subscribeControllerTypes
                .SelectMany(typeInfo => typeInfo.DeclaredMethods)
                .Where(methodInfo => methodInfo.CustomAttributes
                    .Any(attr => typeof(TopicAttribute).IsAssignableFrom(attr.AttributeType)))
                .Select(method => new SubscribeControllerMethod(method))
                .ToList();

            // Return the resulting list of SubscribeControllerMethod instances
            return subscribeControllerMethods;
        }
    }

    //
    // SubscribeController
    //

    public class SubscribeController
    {
    }

    //
    // SubscribeControllerMethod
    //

    public class SubscribeControllerMethod
    {
        public string Topic { get; set; }

        public MethodInfo Handler { get; set; }
        public Type ParameterType { get; set; }
        public Type HandlerClass { get; set; }

        public SubscribeControllerMethod(MethodInfo methodInfo)
        {
            var topicAttribute = methodInfo.GetCustomAttribute<TopicAttribute>(true);

            if (topicAttribute == null)
            {
                throw new NotSupportedException("The topic attribute must be present to allow for a PubSub connection");
            }

            Topic = topicAttribute.TopicName;
            HandlerClass = methodInfo.DeclaringType!;
            Handler = methodInfo;
            ParameterType = methodInfo.GetParameters().Single().ParameterType;
        }
    }

    //
    // TopicAttribute
    //

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class TopicAttribute : Attribute
    {
        public string TopicName { get; set; }

        public TopicAttribute(string topicName)
        {
            TopicName = topicName;
        }
    }
}
