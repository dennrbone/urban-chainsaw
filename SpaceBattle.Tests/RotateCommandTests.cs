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
}