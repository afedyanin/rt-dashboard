using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.Util;
using Microsoft.Extensions.Options;

namespace Dashboard.Application.Artemis
{
    public class ArtemisConsumer
    {
        private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);

        private readonly ArtemisSettings _settings;

        private readonly ConnectionFactory _connectionFactory;

        private IConnection? _connection;

        private Task _consumingTask;

        public ArtemisConsumer(ArtemisSettings settings)
        {
            _settings = settings;
            _connectionFactory = new ConnectionFactory(_settings.ConnectUriString);
        }

        public async Task StartConsume(
            string topic, 
            Func<string, IDictionary<string, string>, CancellationToken, Task> handler,
            CancellationToken cancellationToken)
        {
            // TODO: Use lock fot thread safe
            if (_connection == null)
            {
                _connection = _connectionFactory.CreateConnection(_settings.UserName, _settings.Password);
                _connection.Start();
            }

            using var session = _connection.CreateSession();
            var destination = SessionUtil.GetDestination(session, $"topic://{topic}");
            using var consumer = session.CreateConsumer(destination);


            while (!cancellationToken.IsCancellationRequested)
            {
                // TODO: Should restart wait after timeout
                var message = (ITextMessage)consumer.Receive();

                if (message == null)
                {
                    continue;
                }
                else
                {
                    var props = new Dictionary<string, string>();

                    foreach (string propKey in message.Properties.Keys)
                    {
                        props[propKey] = message.Properties[propKey]?.ToString() ?? "";
                    }

                    await handler.Invoke(message.Text, props, cancellationToken);
                }
            }
        }
    }
}
