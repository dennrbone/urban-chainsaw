using System;
using System.Collections.Generic;
using SpaceBattle.Repositories;
using Xunit;

namespace SpaceBattle.Repositories.Tests;

public class InMemoryGameObjectRepositoryTests
{
    [Fact]
    public void AddAndGet_ShouldSuccessfullyStoreAndRetrieveObject()
    {
        var repository = new InMemoryGameObjectRepository();
        var spaceship = new { Type = "BattleCruiser" };
        string id = "ship-101";

        repository.Add(id, spaceship);
        var retrieved = repository.Get(id);

        Assert.Same(spaceship, retrieved);
    }

    [Fact]
    public void Remove_ShouldDeleteObjectFromRepository()
    {
        var repository = new InMemoryGameObjectRepository();
        var torpedo = new { Damage = 100 };
        string id = "torpedo-505";
        repository.Add(id, torpedo);

        repository.Remove(id);

        Assert.Throws<KeyNotFoundException>(() => repository.Get(id));
    }

    [Fact]
    public void Get_NonExistentId_ShouldThrowKeyNotFoundException()
    {
        var repository = new InMemoryGameObjectRepository();
        string missingId = "ghost-ship";

        Assert.Throws<KeyNotFoundException>(() => repository.Get(missingId));
    }
}