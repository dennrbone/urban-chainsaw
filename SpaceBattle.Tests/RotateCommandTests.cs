using Moq;
using Xunit;

namespace SpaceBattle.Tests;

public class RotateCommandTests
{
    [Fact]
    public void Sum_Rotate()
    {
        var angle = new Angle(45);
        var angularVelocity = new Angle(45);
        var expectedAngle = new Angle(90);

        var rotatingObject = new Mock<IRotatingObject>();
        rotatingObject.Setup(r => r.Angle).Returns(angle);
        rotatingObject.Setup(r => r.AngularVelocity).Returns(angularVelocity);

        var command = new RotateCommand(rotatingObject.Object);
        command.Execute();

        rotatingObject.VerifySet(r => r.Angle = expectedAngle, Times.Once);
    }

    [Fact]
    public void Exception_NoRotate()
    {
        var rotatingObject = new Mock<IRotatingObject>();
        rotatingObject.Setup(r => r.Angle).Throws(new System.Exception());
        rotatingObject.Setup(r => r.AngularVelocity).Returns(new Angle(0));

        var command = new RotateCommand(rotatingObject.Object);

        Assert.Throws<System.Exception>(() => command.Execute());
    }

    [Fact]
    public void Exception_NoVelocityRotate()
    {
        var rotatingObject = new Mock<IRotatingObject>();
        rotatingObject.Setup(r => r.Angle).Returns(new Angle(0));
        rotatingObject.Setup(r => r.AngularVelocity).Throws(new System.Exception());

        var command = new RotateCommand(rotatingObject.Object);

        Assert.Throws<System.Exception>(() => command.Execute());
    }

    [Fact]
    public void Exception_ImpossibleChangeTheLocation()
    {
        var rotatingObject = new Mock<IRotatingObject>();
        rotatingObject.Setup(r => r.Angle).Returns(new Angle(45));
        rotatingObject.Setup(r => r.AngularVelocity).Returns(new Angle(45));
        rotatingObject.SetupSet(r => r.Angle = It.IsAny<Angle>()).Throws(new System.Exception());

        var command = new RotateCommand(rotatingObject.Object);

        Assert.Throws<System.Exception>(() => command.Execute());
    }

    [Fact]
    public void Sum_RotateWithCoord()
    {
        var angle = new Angle(2, 8);
        var angularVelocity = new Angle(2, 8);
        var expectedAngle = new Angle(4, 8);

        var rotatingObject = new Mock<IRotatingObject>();
        rotatingObject.Setup(r => r.Angle).Returns(angle);
        rotatingObject.Setup(r => r.AngularVelocity).Returns(angularVelocity);

        var command = new RotateCommand(rotatingObject.Object);

        command.Execute();

        rotatingObject.VerifySet(r => r.Angle = It.Is<Angle>(a => a.Numerator == expectedAngle.Numerator), Times.Once);
    }


    [Fact]
    public void Sum_Rotate_WithNormalization()
    {
        var angle = new Angle(6, 8);
        var angularVelocity = new Angle(4, 8);
        var expectedAngle = new Angle(2, 8);

        var rotatingObject = new Mock<IRotatingObject>();
        rotatingObject.Setup(r => r.Angle).Returns(angle);
        rotatingObject.Setup(r => r.AngularVelocity).Returns(angularVelocity);

        var command = new RotateCommand(rotatingObject.Object);

        command.Execute();

        rotatingObject.VerifySet(r => r.Angle = It.Is<Angle>(a => a.Numerator == expectedAngle.Numerator), Times.Once);
    }

    [Fact]
    public void RotateCommand_ShouldInitializeCorrectly()
    {
        var rotatingObject = new Mock<IRotatingObject>();

        var command = new RotateCommand(rotatingObject.Object);

        Assert.NotNull(command);
    }

    [Fact]
    public void Execute_ShouldCallGettersOnce()
    {
        var rotatingObject = new Mock<IRotatingObject>();
        rotatingObject.Setup(r => r.Angle).Returns(new Angle(0, 8));
        rotatingObject.Setup(r => r.AngularVelocity).Returns(new Angle(1, 8));

        var command = new RotateCommand(rotatingObject.Object);

        command.Execute();

        rotatingObject.Verify(r => r.Angle, Times.Once);
        rotatingObject.Verify(r => r.AngularVelocity, Times.Once);
    }
}