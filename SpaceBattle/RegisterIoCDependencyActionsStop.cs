namespace SpaceBattle;

public class RegisterIoCDependencyActionsStop : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<ICommand>(
            "IoC.Register",
            "Actions.Stop",
            (Func<object[], object>)(args =>
            {
                var order = (IDictionary<string, object>)args[0];
                var actionId = (string)order["ActionId"];

                if (RegisterIoCDependencyActionsStart._activeActions.TryRemove(actionId, out var injectable))
                {
                    var queue = Ioc.Resolve<ICommandQueue>("Game.Queue");

                    var removeCmd = new ActionCommand(() => queue.Remove((ICommand)injectable));
                    return new SendCommand(removeCmd, queue);
                }

                throw new ArgumentException($"ActionId {actionId} not found");
            })
        ).Execute();
    }
}