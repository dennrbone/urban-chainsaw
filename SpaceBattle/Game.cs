using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceBattle
{
    public class Game
    {
        private readonly IGameObjectRepository _repository;
        private readonly ICommandQueue _commandQueue;
        private bool _isRunning;
        private int _currentStep;

        public Game(IGameObjectRepository repository, ICommandQueue commandQueue)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _commandQueue = commandQueue ?? new SimpleCommandQueue();
        }

        public void SubmitCommand(ICommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            _commandQueue.Enqueue(command);
        }

        public void Update()
        {
            _currentStep++;

            ProcessAllCommands();

            UpdateMovingObjects();
        }

        private void ProcessAllCommands()
        {
            int commandsCount = _commandQueue.Count;

            for (int i = 0; i < commandsCount; i++)
            {
                var command = _commandQueue.Dequeue();
                if (command == null) continue;

                try
                {
                    command.Execute();
                }
                catch (Exception ex)
                {
                    try
                    {
                        var logger = Ioc.Resolve<Action<string>>("Game.Logger.Error");
                        logger?.Invoke($"Ошибка при выполнении команды {command.GetType().Name}: {ex.Message}");
                    }
                    catch
                    {
                    }

                }
            }
        }

        private void UpdateMovingObjects()
        {
            var movingObjects = GetMovingObjects();

            foreach (var movingObject in movingObjects)
            {
                var moveCommand = new MoveCommand(movingObject);
                try
                {
                    moveCommand.Execute();
                }
                catch (Exception ex)
                {
                    try
                    {
                        var logger = Ioc.Resolve<Action<string>>("Game.Logger.Error");
                        logger?.Invoke($"Ошибка движения объекта: {ex.Message}");
                    }
                    catch { }

                }
            }
        }

        private IEnumerable<IMoving> GetMovingObjects()
        {
            var allObjects = _repository.GetAll();

            return allObjects.OfType<IMoving>();
        }
        public void Run(int steps = -1)
        {
            if (steps == -1)
            {
                _isRunning = true;
                while (_isRunning)
                {
                    Update();
                    System.Threading.Thread.Sleep(16);
                }
            }
            else
            {
                for (int i = 0; i < steps; i++)
                {
                    Update();
                }
            }
        }

        public void Stop() => _isRunning = false;
        public int CurrentStep => _currentStep;
        public GameSnapshot GetSnapshot()
        {
            var objects = _repository.GetAll()
                .OfType<IMoving>()
                .Select(obj => new ObjectSnapshot(
                    obj.GetType().Name,
                    obj.Position,
                    obj.Velocity
                ))
                .ToList();

            return new GameSnapshot(_currentStep, objects);
        }
    }
    public class GameSnapshot
    {
        public int Step { get; }
        public IReadOnlyList<ObjectSnapshot> Objects { get; }

        public GameSnapshot(int step, List<ObjectSnapshot> objects)
        {
            Step = step;
            Objects = objects.AsReadOnly();
        }
    }
    public class ObjectSnapshot
    {
        public string Type { get; }
        public NVector Position { get; }
        public NVector Velocity { get; }

        public ObjectSnapshot(string type, NVector position, NVector velocity)
        {
            Type = type;
            Position = position;
            Velocity = velocity;
        }
    }
}