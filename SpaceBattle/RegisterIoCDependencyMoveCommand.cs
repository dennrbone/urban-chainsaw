using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle;

public class RegisterIoCDependencyMoveCommand : ICommand
{
    public void Execute()
    {
        var registerMove = Ioc.Resolve<ICommand>(
            "IoC.Register",
            "Commands.Move",
            (object[] args) =>
            {
                var obj = args[0];
                var movingObject = Ioc.Resolve<IMoving>("Adapters.IMoving", obj);
                return new MoveCommand(movingObject);
            }
        );
        
        registerMove.Execute();
    }
}