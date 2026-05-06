namespace SpaceBattle;

public interface ICommandQueue : ICommandReceiver
{
    void Remove(ICommand command);
}