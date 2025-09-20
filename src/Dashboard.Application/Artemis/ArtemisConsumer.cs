using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.Util;
using Microsoft.Extensions.Options;

namespace Dashboard.Application.Artemis
{
    public class ArtemisConsumer
    {
        private static readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(100);

        private readonly ArtemisSettings _settings;

        private readonly ConnectionFactory _connectionFactory;

        private IConnection? _connection;

        public ArtemisConsumer(ArtemisSettings settings)
        {
            _settings = settings;
            _connectionFactory = new ConnectionFactory(_settings.ConnectUriString);
        }

        public async Task StartConsume(
            string topic, 
            Func<string, IDictionary<string, string>, CancellationToken, ValueTask> handler,
            CancellationToken cancellationToken)
        {
            // TODO: not thread safe!
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
                var message = (ITextMessage) consumer.ReceiveNoWait();

                if (message == null)
                {
                    await Task.Delay(_timeout, cancellationToken);
                    continue;
                }

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
