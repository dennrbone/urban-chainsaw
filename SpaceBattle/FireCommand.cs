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
            // 1. Проверяем авторизацию
            var isAuthorized = Ioc.Resolve<bool>("Authorization.Check", _userId);
            if (!isAuthorized)
            {
                throw new UnauthorizedAccessException($"User {_userId} is not authorized to fire.");
            }

            // 2. Рассчитываем скорость торпеды динамически через стратегии IoC
            // Это обходит отсутствие методов .Normalize() и оператора * внутри класса NVector
            NVector normalizedDir = Ioc.Resolve<NVector>("Vector.Normalize", _direction);
            NVector scaledDir = Ioc.Resolve<NVector>("Vector.Multiply", normalizedDir, _torpedoSpeed);
            NVector torpedoVelocity = Ioc.Resolve<NVector>("Vector.Add", _ship.Velocity, scaledDir);

            // 3. Создаем торпеду как словарь свойств
            var torpedoProperties = new Dictionary<string, object>
            {
                { "Position", _ship.Position },
                { "Velocity", torpedoVelocity }
            };

            // 4. Добавляем торпеду в репозиторий Разработчика 1
            string torpedoId = $"torpedo_{Guid.NewGuid()}";
            _repository.Add(torpedoId, torpedoProperties);

            // 5. Отправляем MoveCommand в очередь игры
            var moveCommand = Ioc.Resolve<ICommand>("Commands.Move", torpedoProperties);
            _commandQueue(moveCommand);
        }
    }
}
