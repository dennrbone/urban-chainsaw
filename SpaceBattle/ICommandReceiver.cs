namespace SpaceBattle;

public interface ICommandReceiver
{
    void Receive(ICommand command);
}