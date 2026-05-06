namespace SpaceBattle;

public interface ICommandInjectable
{
    void Inject(ICommand command);
}