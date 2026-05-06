using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle;
public class RegisterIoCDependencyMacroCommand : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<ICommand>(
            "IoC.Register",
            "Commands.Macro",
            (object[] args) => new MacroCommand((IEnumerable<ICommand>)args[0])
        ).Execute();
    }
}