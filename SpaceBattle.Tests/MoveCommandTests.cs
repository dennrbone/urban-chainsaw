using Xunit;
using Moq;
using SpaceBattle;

public class MoveCommandTests
{
    [Fact]
    public void Move_MovesObjectCorrectly()
    {
        var mock = new Mock<IMoving>();
        mock.SetupProperty(m => m.Position, new NVector(12, 5));
        mock.SetupGet(m => m.Velocity).Returns(new NVector(-4, 1));

        var move = new MoveCommand(mock.Object);
        move.Execute();

        Assert.Equal(new NVector(8, 6), mock.Object.Position);
    }

    [Fact]
    public void Move_ThrowsException_IfPositionUnknown()
    {
        var mock = new Mock<IMoving>();
        mock.SetupGet(m => m.Position).Throws<Exception>();

        var move = new MoveCommand(mock.Object);
        Assert.ThrowsAny<Exception>(() => move.Execute());
    }

    [Fact]
    public void Move_ThrowsException_IfVelocityUnknown()
    {
        var mock = new Mock<IMoving>();
        mock.SetupProperty(m => m.Position, new NVector(12, 5));
        mock.SetupGet(m => m.Velocity).Throws<Exception>();

        var move = new MoveCommand(mock.Object);
        Assert.ThrowsAny<Exception>(() => move.Execute());
    }

    [Fact]
    public void Move_ThrowsException_IfPositionCannotBeSet()
    {
        var mock = new Mock<IMoving>();
        mock.SetupProperty(m => m.Position, new NVector(12, 5));
        mock.SetupGet(m => m.Velocity).Returns(new NVector(1, 1));
        mock.SetupSet(m => m.Position = It.IsAny<NVector>()).Throws<Exception>();

        var move = new MoveCommand(mock.Object);
        Assert.ThrowsAny<Exception>(() => move.Execute());
    }
}