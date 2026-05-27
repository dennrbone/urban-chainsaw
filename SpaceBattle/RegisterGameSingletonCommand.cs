using System;

namespace SpaceBattle
{
    public class RegisterGameSingletonCommand : ICommand
    {
        private readonly IGameObjectRepository _repository;
        private readonly ICommandQueue _commandQueue;
        private static Game _instance;
        private static readonly object _lock = new object();

        public RegisterGameSingletonCommand(IGameObjectRepository repository, ICommandQueue commandQueue)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _commandQueue = commandQueue ?? new SimpleCommandQueue();
        }

        public void Execute()
        {
            var registerGame = Ioc.Resolve<ICommand>(
                "IoC.Register",
                "Game.Instance",
                (object[] args) =>
                {
                    if (_instance == null)
                    {
                        lock (_lock)
                        {
                            if (_instance == null)
                            {
                                _instance = new Game(_repository, _commandQueue);
                            }
                        }
                    }
                    return _instance;
                }
            );

            registerGame.Execute();
        }
    }

    public class RegisterGameDependenciesCommand : ICommand
    {
        public void Execute()
        {
            var registerQueue = Ioc.Resolve<ICommand>(
                "IoC.Register",
                "Game.CommandQueue",
                (object[] args) => new SimpleCommandQueue()
            );
            registerQueue.Execute();

            var repository = Ioc.Resolve<IGameObjectRepository>("Repositories.GameObject");
            var queue = Ioc.Resolve<ICommandQueue>("Game.CommandQueue");

            var registerGame = new RegisterGameSingletonCommand(repository, queue);
            registerGame.Execute();
        }
    }
}
