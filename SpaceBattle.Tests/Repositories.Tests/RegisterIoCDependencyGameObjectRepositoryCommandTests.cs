using System;
using SpaceBattle.Repositories;
using Xunit;

namespace SpaceBattle.Repositories.Tests;

public class RegisterIoCDependencyGameObjectRepositoryCommandTests
{
    [Fact]
    public void Execute_RegisterGameObjectRepository_ResolvesCorrectlyFromIoC()
    {
        var setupCommand = new RegisterIoCDependencyGameObjectRepositoryCommand();
        setupCommand.Execute();

        var repository = Ioc.Resolve<IGameObjectRepository>("Repositories.GameObject", new object());

        Assert.NotNull(repository);

        var alienShip = new { Faction = "Zerg" };
        string id = "alien-01";

        repository.Add(id, alienShip);
        Assert.Same(alienShip, repository.Get(id));

        var secondResolve = Ioc.Resolve<IGameObjectRepository>("Repositories.GameObject", new object());
        Assert.Same(repository, secondResolve);
    }
}