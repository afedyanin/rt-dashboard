namespace Dashboard.Common.Messages
{
    public interface IMessageQueueConsumer
    {
        public bool TryGet(out IDashboardMessage message);
    }
}
