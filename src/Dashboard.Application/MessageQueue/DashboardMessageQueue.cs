using Dashboard.Common.Messages;

namespace Dashboard.Application.MessageQueue
{
    public class DashboardMessageQueue : IMessageQueueProducer, IMessageQueueConsumer
    {
        private readonly PriorityQueue<IDashboardMessage, int> _priorityQueue;

        public DashboardMessageQueue()
        {
            _priorityQueue = new PriorityQueue<IDashboardMessage, int>();
        }

        public void Produce(IDashboardMessage message, int priority)
            => _priorityQueue.Enqueue(message, priority);

        public bool TryGet(out IDashboardMessage message)
            => _priorityQueue.TryDequeue(out message, out _);
    }
}
