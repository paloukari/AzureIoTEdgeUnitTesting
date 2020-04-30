namespace IoTModuleTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using IoTModule;
    using Microsoft.Azure.Devices.Client;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;
    public class EventHandlerModuleTester
    {
        public async Task<ServiceProvider> Init(Action<MessageResponse> messageResultCallback)
        {
            var inputMessageHandlers = new Dictionary<string, (MessageHandler, object)>();

            var services = new ServiceCollection();

            services.AddHostedService<EventHandlerModule>();
            services.AddSingleton((s) =>
            {
                var moduleClient = new Mock<IModuleClient>();

                moduleClient.Setup(e => e.SetInputMessageHandlerAsync(
                    It.IsAny<string>(),
                    It.IsAny<MessageHandler>(),
                    It.IsAny<object>()))
                .Callback<string, MessageHandler, object>((inputName, messageHandler, userContext) =>
                {
                    inputMessageHandlers[inputName] = (messageHandler, userContext);
                }).Returns(Task.FromResult(0));

                moduleClient.Setup(e => e.SendEventAsync(
                    It.IsAny<string>(),
                    It.IsAny<Message>()))
                .Callback<string, Message>((output, message) =>
                {
                    var result = inputMessageHandlers[output].Item1(message,
                                inputMessageHandlers[output].Item2).GetAwaiter().GetResult();

                    messageResultCallback(result);

                }).Returns(Task.FromResult(0));

                return moduleClient.Object;
            });

            services.AddSingleton((s) =>
            {
                var mockLogger = new Mock<ILogger<EventHandlerModule>>();
                return mockLogger.Object;
            });

            var serviceProvider = services.BuildServiceProvider();

            var hostedService = serviceProvider.GetService<IHostedService>();
            await hostedService.StartAsync(new CancellationToken());
            return serviceProvider;
        }

        [Fact]
        public async Task TestInputMessageHandler()
        {
            MessageResponse messageResponse = MessageResponse.Abandoned;
            var serviceProvider = await Init((e) => messageResponse = e);

            await serviceProvider.GetService<IModuleClient>()
                .SendEventAsync("input",
                    new Message(Encoding.UTF8.GetBytes("This is a mocked message!")));

            Assert.Equal(MessageResponse.Completed, messageResponse);
        }
    }
}
