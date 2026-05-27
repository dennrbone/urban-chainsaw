using System;

namespace SpaceBattle
{
    public interface ICommandQueue
    {
        void Enqueue(ICommand command);
        ICommand Dequeue();
        int Count { get; }
        void Clear();
    }
}
