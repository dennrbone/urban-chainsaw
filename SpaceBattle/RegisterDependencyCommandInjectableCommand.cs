namespace SpaceBattle;

public class RegisterDependencyCommandInjectableCommand : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<ICommand>(
            "IoC.Register",
            "Commands.CommandInjectable",
            (Func<object[], object>)(args => new CommandInjectableCommand())
        ).Execute();
    }
}