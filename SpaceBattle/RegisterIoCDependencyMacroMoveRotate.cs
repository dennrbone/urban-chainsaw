using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle;

public class RegisterIoCDependencyMacroMoveRotate : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<ICommand>("IoC.Register", "Macro.Move",
            (object[] args) => new CreateMacroCommandStrategy("Specs.Move").Resolve(args)
        ).Execute();

        Ioc.Resolve<ICommand>("IoC.Register", "Macro.Rotate",
            (object[] args) => new CreateMacroCommandStrategy("Specs.Rotate").Resolve(args)
        ).Execute();
    }
}