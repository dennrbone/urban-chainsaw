using System;
using System.Collections.Generic;

namespace SpaceBattle
{
    public class RegisterIoCDependencyRotateCommand : ICommand
    {
        public void Execute()
        {
            var registerRotate = Ioc.Resolve<ICommand>(
                "IoC.Register",
                "Commands.Rotate",
                (object[] args) =>
                {
                    var obj = args[0];
                    var rotatingObject = Ioc.Resolve<IRotatingObject>("Adapters.IRotatingObject", obj);
                    return new RotateCommand(rotatingObject);
                }
            );

            registerRotate.Execute();
        }
    }
}