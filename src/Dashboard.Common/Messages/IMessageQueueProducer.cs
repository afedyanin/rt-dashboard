namespace Dashboard.Common.Messages
{
    public interface IMessageQueueProducer
    {
        public void Produce(IDashboardMessage message, int priority);
    }
}
