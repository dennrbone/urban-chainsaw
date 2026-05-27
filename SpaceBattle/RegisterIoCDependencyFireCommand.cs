using System;
using System.Collections.Generic;

namespace SpaceBattle
{
    public class RegisterIoCDependencyFireCommand : ICommand
    {
        public void Execute()
        {
            Ioc.Resolve<ICommand>("IoC.Register", "Commands.Fire", (Func<object[], object>)(args =>
            {
                string shipId = (string)args[0];
                NVector direction = (NVector)args[1]; // Приводим строго к NVector
                string userId = (string)args[2];
                double torpedoSpeed = (double)args[3];

                var repository = Ioc.Resolve<IGameObjectRepository>("Repositories.GameObject");
                var queue = Ioc.Resolve<Action<ICommand>>("Game.Queue");

                var shipObj = repository.Get(shipId) as IDictionary<string, object>;
                if (shipObj == null)
                {
                    throw new ArgumentException($"Ship with ID '{shipId}' not found.");
                }

                var movingShip = Ioc.Resolve<IMoving>("Adapters.IMoving", shipObj);
                var authService = Ioc.Resolve<IAuthorizationService>("Services.Authorization");
                return new FireCommand(movingShip, direction, repository, userId, torpedoSpeed, queue, authService);
            })).Execute();
        }
    }
}
