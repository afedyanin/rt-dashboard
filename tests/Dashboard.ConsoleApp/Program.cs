using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.Util;

namespace Dashboard.ConsoleApp
{
    internal class Program
    {
        private static readonly Uri connectUri = new Uri("activemq:tcp://localhost:61616");
        private static readonly string _username = "artemis";
        private static readonly string _password = "test";

        private static TimeSpan _timeout = TimeSpan.FromSeconds(10);

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory(connectUri);
            using var connection = factory.CreateConnection(_username, _password);
            using var session = connection.CreateSession();
            // var destination = SessionUtil.GetDestination(session, "topic://FOO.BAR");
            var destination = SessionUtil.GetDestination(session, "queue://FOO.BAR");
            using var producer = session.CreateProducer(destination);

            connection.Start();
            producer.DeliveryMode = MsgDeliveryMode.Persistent;
            producer.RequestTimeout = _timeout;

            for (int i = 0; i < 10; i++)
            {
                var request = session.CreateTextMessage($"Hello World! #{i}");
                request.NMSCorrelationID = "abc";
                request.Properties["NMSXGroupID"] = "cheese";
                request.Properties["myHeader"] = "Cheddar";

                producer.Send(request);
            }
        }
    }
}
