using Xunit;
using Moq;
using System;
using System.Collections.Generic;

namespace SpaceBattle.Tests;

public class FireCommandTests
{
    public FireCommandTests()
    {
        Ioc.Resolve<ICommand>("IoC.Register", "Authorization.Check",
            (object[] args) => (object)true).Execute();

        Ioc.Resolve<ICommand>("IoC.Register", "Vector.Normalize",
            (object[] args) => args[0]).Execute();

        Ioc.Resolve<ICommand>("IoC.Register", "Vector.Multiply",
            (object[] args) =>
            {
                var vec = (NVector)args[0];
                double multiplier = (double)args[1];
                var coords = vec.Coords.Select(c => (int)(c * multiplier)).ToArray();
                return (object)new NVector(coords);
            }).Execute();

        Ioc.Resolve<ICommand>("IoC.Register", "Vector.Add",
            (object[] args) =>
            {
                var a = (NVector)args[0];
                var b = (NVector)args[1];
                return (object)(a + b);
            }).Execute();

        Ioc.Resolve<ICommand>("IoC.Register", "Commands.Move",
            (object[] args) => (object)new Mock<ICommand>().Object).Execute();
    }

    [Fact]
    public void Execute_WhenAuthorized_CreatesTorpedoAndQueuesMoveCommand()
    {
        var ship = new Mock<IMoving>();
        var position = new NVector(10, 20, 30);
        var velocity = new NVector(1, 2, 3);
        ship.SetupGet(s => s.Position).Returns(position);
        ship.SetupGet(s => s.Velocity).Returns(velocity);

        var direction = new NVector(1, 0, 0);
        var repository = new Mock<IGameObjectRepository>();
        var commandQueue = new Mock<Action<ICommand>>();
        var userId = "user123";
        int torpedoSpeed = 5;

        var fireCommand = new FireCommand(
            ship.Object,
            direction,
            repository.Object,
            userId,
            torpedoSpeed,
            commandQueue.Object
        );

        fireCommand.Execute();

        repository.Verify(r => r.Add(It.Is<string>(id => id.StartsWith("torpedo_")),
            It.Is<Dictionary<string, object>>(props =>
                props["Position"].Equals(position) &&
                ((NVector)props["Velocity"]).Coords[0] == 6 &&
                ((NVector)props["Velocity"]).Coords[1] == 2 &&
                ((NVector)props["Velocity"]).Coords[2] == 3
            )), Times.Once);

        commandQueue.Verify(q => q(It.IsAny<ICommand>()), Times.Once);
    }

    [Fact]
    public void Constructor_WhenShipIsNull_ThrowsArgumentNullException()
    {
        var direction = new NVector(1, 0, 0);
        var repository = new Mock<IGameObjectRepository>().Object;
        var commandQueue = new Mock<Action<ICommand>>().Object;
        var userId = "user123";
        int torpedoSpeed = 5;

        Assert.Throws<ArgumentNullException>(() => new FireCommand(
            null,
            direction,
            repository,
            userId,
            torpedoSpeed,
            commandQueue
        ));
    }

    [Fact]
    public void Constructor_WhenDirectionIsNull_ThrowsArgumentNullException()
    {
        var ship = new Mock<IMoving>().Object;
        var repository = new Mock<IGameObjectRepository>().Object;
        var commandQueue = new Mock<Action<ICommand>>().Object;
        var userId = "user123";
        int torpedoSpeed = 5;

        Assert.Throws<ArgumentNullException>(() => new FireCommand(
            ship,
            null,
            repository,
            userId,
            torpedoSpeed,
            commandQueue
        ));
    }

    [Fact]
    public void Constructor_WhenRepositoryIsNull_ThrowsArgumentNullException()
    {
        var ship = new Mock<IMoving>().Object;
        var direction = new NVector(1, 0, 0);
        var commandQueue = new Mock<Action<ICommand>>().Object;
        var userId = "user123";
        int torpedoSpeed = 5;

        Assert.Throws<ArgumentNullException>(() => new FireCommand(
            ship,
            direction,
            null,
            userId,
            torpedoSpeed,
            commandQueue
        ));
    }

    [Fact]
    public void Constructor_WhenUserIdIsNull_ThrowsArgumentNullException()
    {
        var ship = new Mock<IMoving>().Object;
        var direction = new NVector(1, 0, 0);
        var repository = new Mock<IGameObjectRepository>().Object;
        var commandQueue = new Mock<Action<ICommand>>().Object;
        int torpedoSpeed = 5;

        Assert.Throws<ArgumentNullException>(() => new FireCommand(
            ship,
            direction,
            repository,
            null,
            torpedoSpeed,
            commandQueue
        ));
    }

    [Fact]
    public void Constructor_WhenCommandQueueIsNull_ThrowsArgumentNullException()
    {
        var ship = new Mock<IMoving>().Object;
        var direction = new NVector(1, 0, 0);
        var repository = new Mock<IGameObjectRepository>().Object;
        var userId = "user123";
        int torpedoSpeed = 5;

        Assert.Throws<ArgumentNullException>(() => new FireCommand(
            ship,
            direction,
            repository,
            userId,
            torpedoSpeed,
            null
        ));
    }

    [Fact]
    public void Execute_VectorNormalize_IsCalledWithDirection()
    {
        bool normalizeCalled = false;
        Ioc.Resolve<ICommand>("IoC.Register", "Vector.Normalize",
            (object[] args) =>
            {
                normalizeCalled = true;
                return args[0];
            }).Execute();

        var ship = new Mock<IMoving>();
        ship.SetupGet(s => s.Position).Returns(new NVector(0, 0, 0));
        ship.SetupGet(s => s.Velocity).Returns(new NVector(0, 0, 0));

        var direction = new NVector(1, 1, 0);
        var repository = new Mock<IGameObjectRepository>();
        var commandQueue = new Mock<Action<ICommand>>();
        var userId = "user123";
        int torpedoSpeed = 5;

        var fireCommand = new FireCommand(
            ship.Object,
            direction,
            repository.Object,
            userId,
            torpedoSpeed,
            commandQueue.Object
        );

        fireCommand.Execute();

        Assert.True(normalizeCalled);
    }

    [Fact]
    public void Execute_VectorMultiply_IsCalledWithCorrectParameters()
    {
        // Arrange
        NVector receivedVector = null;
        double receivedSpeed = 0;

        Ioc.Resolve<ICommand>("IoC.Register", "Vector.Multiply",
            (object[] args) =>
            {
                receivedVector = (NVector)args[0];
                receivedSpeed = (double)args[1];
                var coords = receivedVector.Coords.Select(c => (int)(c * receivedSpeed)).ToArray();
                return (object)new NVector(coords);
            }).Execute();

        var ship = new Mock<IMoving>();
        ship.SetupGet(s => s.Position).Returns(new NVector(0, 0, 0));
        ship.SetupGet(s => s.Velocity).Returns(new NVector(0, 0, 0));

        var direction = new NVector(2, 3, 4);
        var repository = new Mock<IGameObjectRepository>();
        var commandQueue = new Mock<Action<ICommand>>();
        var userId = "user123";
        double torpedoSpeed = 7.0;

        var fireCommand = new FireCommand(
            ship.Object,
            direction,
            repository.Object,
            userId,
            torpedoSpeed,
            commandQueue.Object
        );

        fireCommand.Execute();

        Assert.Equal(direction.Coords, receivedVector.Coords);
        Assert.Equal(torpedoSpeed, receivedSpeed);
    }

    [Fact]
    public void Execute_GeneratesUniqueTorpedoId()
    {
        var capturedIds = new List<string>();
        var repository = new Mock<IGameObjectRepository>();
        repository.Setup(r => r.Add(It.IsAny<string>(), It.IsAny<object>()))
            .Callback<string, object>((id, props) => capturedIds.Add(id));

        var ship = new Mock<IMoving>();
        ship.SetupGet(s => s.Position).Returns(new NVector(0, 0, 0));
        ship.SetupGet(s => s.Velocity).Returns(new NVector(0, 0, 0));

        var direction = new NVector(1, 0, 0);
        var commandQueue = new Mock<Action<ICommand>>();
        var userId = "user123";
        int torpedoSpeed = 5;

        var fireCommand1 = new FireCommand(
            ship.Object,
            direction,
            repository.Object,
            userId,
            torpedoSpeed,
            commandQueue.Object
        );

        var fireCommand2 = new FireCommand(
            ship.Object,
            direction,
            repository.Object,
            userId,
            torpedoSpeed,
            commandQueue.Object
        );

        fireCommand1.Execute();
        fireCommand2.Execute();

        Assert.Equal(2, capturedIds.Count);
        Assert.NotEqual(capturedIds[0], capturedIds[1]);
    }
}