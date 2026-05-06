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
}
