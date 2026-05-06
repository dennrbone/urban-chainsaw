namespace SpaceBattle;

public class CommandInjectableCommand : ICommand, ICommandInjectable
{
    private ICommand? _command;

    public void Inject(ICommand command) => _command = command;

    public void Execute()
    {
        if (_command == null)
        {
            throw new InvalidOperationException("Команда не была внедрена (Injected)");
        }
        _command.Execute();
    }
}