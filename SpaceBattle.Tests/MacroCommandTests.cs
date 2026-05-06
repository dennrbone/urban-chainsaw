using System;
using Moq;
using Xunit;

namespace SpaceBattle.Tests
{
    public class MacroCommandTests
    {
        [Fact]
        public void MacroCommand_ShouldStop_WhenCommandThrows()
        {
            var cmd1 = new Mock<ICommand>();
            var cmd2 = new Mock<ICommand>();
            cmd1.Setup(c => c.Execute()).Throws<Exception>();

            var macro = new MacroCommand(new[] { cmd1.Object, cmd2.Object });

            Assert.ThrowsAny<Exception>(() => macro.Execute());
            cmd2.Verify(c => c.Execute(), Times.Never());
        }
    }
}