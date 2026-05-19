using System;
using System.Collections.Generic;

namespace SpaceBattle
{
    public class FireCommand : ICommand
    {
        private readonly IMoving _ship;
        private readonly NVector _direction;
        private readonly IGameObjectRepository _repository;
        private readonly string _userId;
        private readonly double _torpedoSpeed;
        private readonly Action<ICommand> _commandQueue;

        public FireCommand(
            IMoving ship,
            NVector direction,
            IGameObjectRepository repository,
            string userId,
            double torpedoSpeed,
            Action<ICommand> commandQueue)
        {
            _ship = ship ?? throw new ArgumentNullException(nameof(ship));
            _direction = direction ?? throw new ArgumentNullException(nameof(direction));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _userId = userId ?? throw new ArgumentNullException(nameof(userId));
            _torpedoSpeed = torpedoSpeed;
            _commandQueue = commandQueue ?? throw new ArgumentNullException(nameof(commandQueue));
        }

        public void Execute()
        {
            var isAuthorized = Ioc.Resolve<bool>("Authorization.Check", _userId);
            if (!isAuthorized)
            {
                throw new UnauthorizedAccessException($"User {_userId} is not authorized to fire.");
            }


            NVector normalizedDir = Ioc.Resolve<NVector>("Vector.Normalize", _direction);
            NVector scaledDir = Ioc.Resolve<NVector>("Vector.Multiply", normalizedDir, _torpedoSpeed);
            NVector torpedoVelocity = Ioc.Resolve<NVector>("Vector.Add", _ship.Velocity, scaledDir);

            var torpedoProperties = new Dictionary<string, object>
            {
                { "Position", _ship.Position },
                { "Velocity", torpedoVelocity }
            };

            string torpedoId = $"torpedo_{Guid.NewGuid()}";
            _repository.Add(torpedoId, torpedoProperties);

            var moveCommand = Ioc.Resolve<ICommand>("Commands.Move", torpedoProperties);
            _commandQueue(moveCommand);
        }
    }
}
