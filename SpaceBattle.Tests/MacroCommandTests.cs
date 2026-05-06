using System;
using System.Collections.Generic;
using Moq;
using Xunit;
using System.Linq;

namespace SpaceBattle.Tests
{
    public class MacroCommandTests
    {
        [Fact]
        public void MacroCommand_ShouldExecuteAllCommands_InCorrectOrder()
        {
            var sequence = new List<int>();
            var cmd1 = new Mock<ICommand>();
            cmd1.Setup(c => c.Execute()).Callback(() => sequence.Add(1));

            var cmd2 = new Mock<ICommand>();
            cmd2.Setup(c => c.Execute()).Callback(() => sequence.Add(2));

            var macro = new MacroCommand(new[] { cmd1.Object, cmd2.Object });

            macro.Execute();

            Assert.Equal(new[] { 1, 2 }, sequence);
            cmd1.Verify(c => c.Execute(), Times.Once());
            cmd2.Verify(c => c.Execute(), Times.Once());
        }

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

        [Fact]
        public void MacroCommand_WithEmptyList_ShouldDoNothing()
        {
            var macro = new MacroCommand(Enumerable.Empty<ICommand>());

            var exception = Record.Exception(() => macro.Execute());

            Assert.Null(exception);
        }


        [Fact]
        public void MacroCommand_ShouldExecuteAllCommands_WhenNoExceptions()
        {
            var cmd1 = new Mock<ICommand>();
            var cmd2 = new Mock<ICommand>();
            var macro = new MacroCommand(new[] { cmd1.Object, cmd2.Object });

            macro.Execute();

            cmd1.Verify(c => c.Execute(), Times.Once());
            cmd2.Verify(c => c.Execute(), Times.Once());
        }
    }
}
