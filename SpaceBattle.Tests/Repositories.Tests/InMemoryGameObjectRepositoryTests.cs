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

    [Fact]
    public void Add_WithEmptyIdOrNullObject_ShouldThrowArgumentExceptions()
    {
        var repository = new InMemoryGameObjectRepository();

        Assert.Throws<ArgumentException>(() => repository.Add("", new object()));
        Assert.Throws<ArgumentException>(() => repository.Add("   ", new object()));

        Assert.Throws<ArgumentNullException>(() => repository.Add("valid-id", null!));
    }

    [Fact]
    public void Add_DuplicateId_ShouldThrowInvalidOperationException()
    {
        var repository = new InMemoryGameObjectRepository();
        var obj1 = new object();
        var obj2 = new object();
        string id = "duplicate-id";

        repository.Add(id, obj1);

        Assert.Throws<InvalidOperationException>(() => repository.Add(id, obj2));
    }

    [Fact]
    public void Remove_WithEmptyId_ShouldThrowArgumentException()
    {
        var repository = new InMemoryGameObjectRepository();

        Assert.Throws<ArgumentException>(() => repository.Remove(""));
    }

    [Fact]
    public void GetAll_ShouldReturnAllStoredObjects()
    {
        var repository = new InMemoryGameObjectRepository();
        var obj1 = new object();
        var obj2 = new object();

        repository.Add("id-1", obj1);
        repository.Add("id-2", obj2);

        var allObjects = repository.GetAll();

        Assert.Contains(obj1, allObjects);
        Assert.Contains(obj2, allObjects);
    }
}