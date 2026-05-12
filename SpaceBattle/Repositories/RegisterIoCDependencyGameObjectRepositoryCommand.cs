using System;
using SpaceBattle;

namespace SpaceBattle.Repositories;

public class RegisterIoCDependencyGameObjectRepositoryCommand : ICommand
{
    public void Execute()
    {
        var gameObjectRepository = new InMemoryGameObjectRepository();

        var registerRepository = Ioc.Resolve<ICommand>(
            "IoC.Register",
            "Repositories.GameObject",
            (object[] args) =>
            {
                return gameObjectRepository;
            }
        );

        registerRepository.Execute();
    }
}