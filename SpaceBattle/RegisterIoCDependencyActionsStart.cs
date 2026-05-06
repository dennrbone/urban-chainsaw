using System.Collections.Concurrent;

namespace SpaceBattle;

public class RegisterIoCDependencyActionsStart : ICommand
{
    public static readonly ConcurrentDictionary<string, ICommandInjectable> _activeActions = new();

    public void Execute()
    {
        Ioc.Resolve<ICommand>(
            "IoC.Register",
            "Actions.Start",
            (Func<object[], object>)(args =>
            {
                var order = (IDictionary<string, object>)args[0];
                var actionId = (string)order["ActionId"];
                var obj = order["GameObject"];
                var cmdName = (string)order["CommandName"];

                var realCommand = Ioc.Resolve<ICommand>(cmdName, obj);

                var injectable = new CommandInjectableCommand();
                injectable.Inject(realCommand);

                _activeActions[actionId] = injectable;

                var queue = Ioc.Resolve<ICommandReceiver>("Game.Queue");
                return new SendCommand((ICommand)injectable, queue);
            })
        ).Execute();
    }
}