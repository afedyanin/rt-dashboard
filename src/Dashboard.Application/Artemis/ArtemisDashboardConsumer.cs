using Dashboard.Common.Messages;

namespace Dashboard.Application.Artemis
{
    public class ArtemisDashboardConsumer : ArtemisConsumer
    {
        private readonly IMessageQueueProducer _messageQueueProducer;

        public ArtemisDashboardConsumer(
            IMessageQueueProducer messageQueueProducer,
            ArtemisSettings settings) : base(settings)
        {
            _messageQueueProducer = messageQueueProducer;
        }

        public Task StartConsume(
            string topic, CancellationToken cancellationToken)
            => StartConsume(topic, Handle, cancellationToken);

        private ValueTask Handle(
            string message, 
            IDictionary<string, string> props, 
            CancellationToken cancellationToken)
        {
            if (!props.TryGetValue("Type", out var messageType))
            {
                return ValueTask.CompletedTask;
            }

            var dashboardMessage = DashboardMessageFactory.Create(message, messageType);

            if (dashboardMessage == null)
            {
                return ValueTask.CompletedTask;
            }

            _messageQueueProducer.Produce(dashboardMessage, dashboardMessage.Priority);

            return ValueTask.CompletedTask;
        }
    }
}
