using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bedrock.Framework;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BedrockWebSocketTest.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var server = new ServerBuilder(BuildServiceProvider())
                .ListenWebSocket(new Uri("wss://localhost:1337"), x =>
                {
                    x.UseConnectionHandler<SessionHandler>();
                })
                .Build();

            await server.StartAsync();

            var tcs = new TaskCompletionSource<object>();
            Console.CancelKeyPress += (sender, e) => tcs.TrySetResult(true);
            await tcs.Task;
        }

        public static IServiceProvider BuildServiceProvider()
        {
            var builder = new ContainerBuilder();
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddLogging(b =>
            {
                b.AddConsole();
            });

            builder.RegisterInstance(new LoggerFactory())
                .As<ILoggerFactory>();

            builder.RegisterGeneric(typeof(Logger<>))
                .As(typeof(ILogger<>))
                .SingleInstance();

            builder.Populate(serviceCollection);

            return new AutofacServiceProvider(builder.Build());
        }
    }
}
