using Moq;
using Xunit;
using SpaceBattle;

namespace SpaceBattle.Tests;

public class SendCommandTests
{
    [Fact]
    public void SendCommand_PassesCommandToReceiver()
    {
        var mockCommand = new Mock<ICommand>();
        var mockReceiver = new Mock<ICommandReceiver>();

        var sendCommand = new SendCommand(mockCommand.Object, mockReceiver.Object);
        sendCommand.Execute();

        mockReceiver.Verify(r => r.Receive(mockCommand.Object), Times.Once);
    }

    [Fact]
    public void SendCommand_WhenReceiverFails_ThrowsException()
    {
        var mockCommand = new Mock<ICommand>();
        var mockReceiver = new Mock<ICommandReceiver>();
        mockReceiver.Setup(r => r.Receive(It.IsAny<ICommand>())).Throws(new Exception());

        var sendCommand = new SendCommand(mockCommand.Object, mockReceiver.Object);

        Assert.Throws<Exception>(() => sendCommand.Execute());
    }

    [Fact]
    public void SendCommand_ShouldStoreCommandReference()
    {
        var mockCommand = new Mock<ICommand>();
        var mockReceiver = new Mock<ICommandReceiver>();

        var sendCommand = new SendCommand(mockCommand.Object, mockReceiver.Object);
        sendCommand.Execute();

        mockReceiver.Verify(r => r.Receive(It.Is<ICommand>(c => c == mockCommand.Object)), Times.Once);
    }

    [Fact]
    public void SendCommand_ShouldNotExecute()
    {
        var mockCommand = new Mock<ICommand>();
        var mockReceiver = new Mock<ICommandReceiver>();

        var sendCommand = new SendCommand(mockCommand.Object, mockReceiver.Object);
        sendCommand.Execute();

        mockCommand.Verify(c => c.Execute(), Times.Never);
    }

    [Fact]
    public void SendCommand_WithNullReceiver()
    {
        var mockCommand = new Mock<ICommand>();

        var sendCommand = new SendCommand(mockCommand.Object, null);

        Assert.Throws<NullReferenceException>(() => sendCommand.Execute());
    }

    [Fact]
    public void SendCommand_ExecuteMultipleCalls()
    {
        var mockCommand = new Mock<ICommand>();
        var mockReceiver = new Mock<ICommandReceiver>();

        var sendCommand = new SendCommand(mockCommand.Object, mockReceiver.Object);

        sendCommand.Execute();
        sendCommand.Execute();
        sendCommand.Execute();

        mockReceiver.Verify(r => r.Receive(mockCommand.Object), Times.Exactly(3));
    }
}
