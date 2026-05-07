namespace SpaceBattle
{
    public class RegisterIoCDependencySendCommand : ICommand
    {
        public void Execute()
        {
            var registerSend = Ioc.Resolve<ICommand>(
                "IoC.Register",
                "Commands.Send",
                (object[] args) =>
                {
                    var command = (ICommand)args[0];
                    var receiver = (ICommandReceiver)args[1];
                    return new SendCommand(command, receiver);
                }
            );

            registerSend.Execute();
        }
    }
}
