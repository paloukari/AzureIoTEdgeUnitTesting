namespace IoTModule
{
    using Microsoft.Azure.Devices.Client;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class EventHandlerModule : IHostedService
    {
        readonly IModuleClient moduleClient;
        readonly ILogger logger;

        public EventHandlerModule(IModuleClient moduleClient,
            ILogger<EventHandlerModule> logger)
        {
            this.moduleClient = moduleClient;
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await moduleClient.OpenAsync(cancellationToken);

            await moduleClient.SetInputMessageHandlerAsync("input",
                new MessageHandler(async (message, context) =>
                {
                    var messageText = Encoding.UTF8.GetString(message.GetBytes());
                    logger.LogInformation($"Message received: {messageText}");
                    return await Task.FromResult(MessageResponse.Completed);
                }), this);

            logger.LogInformation("Started.");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await moduleClient.CloseAsync(cancellationToken);
            logger.LogInformation("Stopped.");
        }
    }
}