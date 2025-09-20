using Dashboard.Common.Messages;

namespace Dashboard.Application.MessageQueue
{
    public class DashboardMessageQueue : IMessageQueueProducer, IMessageQueueConsumer
    {
        private readonly PriorityQueue<IDashboardMessage, int> _priorityQueue;
        private readonly object _lock = new object();

        public DashboardMessageQueue()
        {
            _priorityQueue = new PriorityQueue<IDashboardMessage, int>();
        }

        public void Produce(IDashboardMessage message, int priority)
        {
            lock (_lock)
            {
                _priorityQueue.Enqueue(message, priority);
            }
        }

        public bool TryGet(out IDashboardMessage message)
        {
            lock (_lock)
            {
                return _priorityQueue.TryDequeue(out message, out _);
            }
        }
    }
}
