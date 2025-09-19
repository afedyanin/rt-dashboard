using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.Util;
using Microsoft.Extensions.Options;

namespace Dashboard.Application.Artemis
{
    public class ArtemisProducer : IDisposable
    {
        private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);

        private readonly ArtemisSettings _settings;

        private readonly ConnectionFactory _connectionFactory;

        private IConnection? _connection;

        public ArtemisProducer(ArtemisSettings settings)
        {
            _settings = settings;
            _connectionFactory = new ConnectionFactory(_settings.ConnectUriString);
        }

        public void Produce(string topic, string message, IDictionary<string, string>? props = null)
            => Produce(topic, [message], props);

        public void Produce(string topic, string[] messages, IDictionary<string, string>? props = null)
        {
            // TODO: Use lock fot thread safe
            if (_connection == null)
            {
                _connection = _connectionFactory.CreateConnection(_settings.UserName, _settings.Password);
                _connection.Start();
            }

            using var session = _connection.CreateSession();
            var destination = SessionUtil.GetDestination(session, $"topic://{topic}");
            using var producer = session.CreateProducer(destination);

            producer.DeliveryMode = MsgDeliveryMode.NonPersistent;
            producer.RequestTimeout = _timeout;

            foreach (var message in messages)
            {
                if (string.IsNullOrEmpty(message))
                {
                    continue;
                }

                var request = session.CreateTextMessage(message);

                if (props != null)
                {
                    foreach (var kvp in props)
                    {
                        request.Properties[kvp.Key] = kvp.Value;
                    }
                }

                producer.Send(request);
            }
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
