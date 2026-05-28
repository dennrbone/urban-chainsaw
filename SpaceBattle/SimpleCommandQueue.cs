using System.Collections.Concurrent;

namespace SpaceBattle
{
    public class SimpleCommandQueue : ICommandQueue
    {
        private readonly ConcurrentQueue<ICommand> _queue = new();

        public void Enqueue(ICommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            _queue.Enqueue(command);
        }

        public ICommand Dequeue()
        {
            _queue.TryDequeue(out var command);
            return command;
        }

        public int Count => _queue.Count;

        public void Clear()
        {
            while (_queue.TryDequeue(out _)) { }
        }
    }
}
