using Xunit;
using Moq;

namespace SpaceBattle.Tests;

public class RegisterIoCDependencyRotateCommandTests
{
    [Fact]
    public void Execute_RegisterIoCDependencyRotateCommand_DependencyResolves()
    {
        var mockRotating = new Mock<IRotatingObject>();

        mockRotating.SetupProperty(r => r.Angle, new Angle(45));
        mockRotating.SetupGet(r => r.AngularVelocity).Returns(new Angle(45));

        Ioc.Resolve<ICommand>(
            "IoC.Register",
            "Adapters.IRotatingObject",
            (object[] args) => mockRotating.Object
        ).Execute();

        new RegisterIoCDependencyRotateCommand().Execute();

        var cmd = Ioc.Resolve<ICommand>("Commands.Rotate", new object());

        Assert.NotNull(cmd);

        cmd.Execute();

        Assert.Equal(new Angle(90), mockRotating.Object.Angle);
    }
}