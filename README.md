# Advanced .NET Technique: Custom Attributes for PubSub Controllers

This project demonstrates an advanced usage of Custom Attributes in .NET to create a Publish-Subscribe system, similar to how REST API controllers function. [Published in an article on Medium.](https://medium.com/quick-code/advanced-net-technique-custom-attributes-821d89826422)

## Overview

.NET Custom Attributes allow for declarative metadata annotations on class elements, which can be introspected at runtime using reflection. This project showcases their utility in dynamically wiring up a pub-sub message handling system.

## Features

- **Publisher**: A component responsible for broadcasting messages over a named pipe.
- **PubSubExtension**: An extension class for IServiceCollection that autocovers and registers methods with the `[Topic]` attribute.
- **Subscriber**: A background service that listens for messages and triggers corresponding methods marked by the `[Topic]` attribute.
- **SubscribeController**: Base class for controllers with methods that can be invoked through pub-sub messaging.

## Getting Started

To explore this example on your machine:

1. Clone the repository.
2. Ensure you have the .NET development environment set up.
3. Build and run the project in your preferred .NET-compatible IDE (like Visual Studio).

Start the Subscriber:
```
cd Subscriber
dotnet run
```

Start the Publisher
```
cd Publisher
dotnet run

# => start typing a message and send it by pressing 'Enter'
# * the Subscriber will receive the message
```


## Usage

- Example `Publish` method invocation:

```csharp
var publisher = new Publisher();
publisher.Publish("YourTopic", new YourMessageType { /* message data */ });
```

- Create a `SubscribeController` derived class:

```csharp
public class YourController : SubscribeController
{
    [Topic("YourTopic")]
    public async Task<object> YourMessageHandler(YourMessageType message)
    {
        // Handle incoming message
    }
}
```

Ensure your derived controller class and its message-handling methods are accessible to the `PubSubExtension` logic.

- Integrate the pub-sub mechanism into your application's startup:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddPubSub(/* ApplicationName */);
}
```

## Contributions

Feel free to fork, modify, and use this codebase as you see fit.

## Resources

- For more clean code principles and practices, visit [Clean Code Principles](https://cln.co).

