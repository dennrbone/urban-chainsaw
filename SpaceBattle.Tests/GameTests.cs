using Xunit;
using System;
using System.Collections.Generic;
using SpaceBattle.Repositories;

namespace SpaceBattle.Tests
{
    public class GameTests
    {
        [Fact]
        public void Torpedo_Moves_After_Update()
        {
            var repository = new InMemoryGameObjectRepository();
            var queue = new SimpleCommandQueue();
            var game = new Game(repository, queue);

            var torpedo = new TestTorpedo("torp1", new NVector(0, 0), new NVector(5, 0));
            repository.Add("torp1", torpedo);

            game.Update();

            Assert.Equal(new NVector(5, 0), torpedo.Position);
        }

        [Fact]
        public void MultipleCommands_Executed_InCorrectOrder()
        {
            var repository = new InMemoryGameObjectRepository();
            var queue = new SimpleCommandQueue();
            var game = new Game(repository, queue);

            var executionOrder = new List<string>();

            var command1 = new TestCommand(() => executionOrder.Add("Command1"));
            var command2 = new TestCommand(() => executionOrder.Add("Command2"));
            var command3 = new TestCommand(() => executionOrder.Add("Command3"));

            game.SubmitCommand(command1);
            game.SubmitCommand(command2);
            game.SubmitCommand(command3);
            game.Update();

            Assert.Equal(new[] { "Command1", "Command2", "Command3" }, executionOrder);
        }

        [Fact]
        public void Game_Continues_After_CommandException()
        {
            var repository = new InMemoryGameObjectRepository();
            var queue = new SimpleCommandQueue();
            var game = new Game(repository, queue);

            var errorCommand = new ErrorCommand(new Exception("Test error"));
            var normalCommand = new TestCommand(() => { });

            game.SubmitCommand(errorCommand);
            game.SubmitCommand(normalCommand);

            var exception = Record.Exception(() => game.Update());

            Assert.Null(exception);
        }

        [Fact]
        public void Game_Continues_After_MoveException()
        {
            var repository = new InMemoryGameObjectRepository();
            var queue = new SimpleCommandQueue();
            var game = new Game(repository, queue);

            var brokenTorpedo = new BrokenTorpedo("broken", new NVector(0, 0));
            repository.Add("broken", brokenTorpedo);

            var normalTorpedo = new TestTorpedo("normal", new NVector(0, 0), new NVector(1, 0));
            repository.Add("normal", normalTorpedo);

            var exception = Record.Exception(() => game.Update());

            Assert.Null(exception);
            Assert.Equal(new NVector(1, 0), normalTorpedo.Position);
            Assert.Equal(new NVector(0, 0), brokenTorpedo.Position);
        }

        [Fact]
        public void Game_Singleton_Registration_Works()
        {
            var repository = new InMemoryGameObjectRepository();
            var queue = new SimpleCommandQueue();

            var registerGame = new RegisterGameSingletonCommand(repository, queue);
            registerGame.Execute();

            var game1 = Ioc.Resolve<Game>("Game.Instance");
            var game2 = Ioc.Resolve<Game>("Game.Instance");

            Assert.Same(game1, game2);
        }

        [Fact]
        public void MultipleUpdates_AccumulateMovement()
        {
            var repository = new InMemoryGameObjectRepository();
            var queue = new SimpleCommandQueue();
            var game = new Game(repository, queue);

            var torpedo = new TestTorpedo("torp1", new NVector(0, 0), new NVector(5, 3));
            repository.Add("torp1", torpedo);

            game.Update();
            game.Update();
            game.Update();

            Assert.Equal(new NVector(15, 9), torpedo.Position);
        }

        [Fact]
        public void CommandsAndMovement_ExecuteInCorrectOrder()
        {
            var repository = new InMemoryGameObjectRepository();
            var queue = new SimpleCommandQueue();
            var game = new Game(repository, queue);

            var torpedo = new TestTorpedo("torp1", new NVector(0, 0), new NVector(5, 0));
            repository.Add("torp1", torpedo);

            var positionBeforeMove = new NVector(0, 0);
            var executionLog = new List<string>();

            var checkPositionCommand = new TestCommand(() =>
            {
                executionLog.Add($"ChangeVelocity at position {torpedo.Position}");
                Assert.Equal(positionBeforeMove, torpedo.Position);
            });

            game.SubmitCommand(checkPositionCommand);
            game.Update();

            Assert.Equal(new NVector(5, 0), torpedo.Position);
        }

        [Fact]
        public void SubmitCommand_AddsCommandToQueue()
        {
            var repository = new InMemoryGameObjectRepository();
            var queue = new SimpleCommandQueue();
            var game = new Game(repository, queue);

            var command = new TestCommand(() => { });

            game.SubmitCommand(command);

            Assert.Equal(1, queue.Count);
        }

        [Fact]
        public void Update_ProcessesAllCommandsEvenIfSomeFail()
        {
            var repository = new InMemoryGameObjectRepository();
            var queue = new SimpleCommandQueue();
            var game = new Game(repository, queue);

            var executionCount = 0;
            var command1 = new ErrorCommand(new Exception("First error"));
            var command2 = new TestCommand(() => executionCount++);
            var command3 = new ErrorCommand(new Exception("Second error"));
            var command4 = new TestCommand(() => executionCount++);

            game.SubmitCommand(command1);
            game.SubmitCommand(command2);
            game.SubmitCommand(command3);
            game.SubmitCommand(command4);
            game.Update();

            Assert.Equal(2, executionCount);
        }

        [Fact]
        public void Update_MovesAllMovingObjectsEvenIfSomeFail()
        {
            var repository = new InMemoryGameObjectRepository();
            var queue = new SimpleCommandQueue();
            var game = new Game(repository, queue);

            var torpedo1 = new TestTorpedo("torp1", new NVector(0, 0), new NVector(1, 0));
            var brokenTorpedo = new BrokenTorpedo("broken", new NVector(0, 0));
            var torpedo2 = new TestTorpedo("torp2", new NVector(0, 0), new NVector(2, 0));

            repository.Add("torp1", torpedo1);
            repository.Add("broken", brokenTorpedo);
            repository.Add("torp2", torpedo2);

            game.Update();

            Assert.Equal(new NVector(1, 0), torpedo1.Position);
            Assert.Equal(new NVector(2, 0), torpedo2.Position);
            Assert.Equal(new NVector(0, 0), brokenTorpedo.Position);
        }

        [Fact]
        public void Run_ExecutesSpecifiedNumberOfSteps()
        {
            var repository = new InMemoryGameObjectRepository();
            var queue = new SimpleCommandQueue();
            var game = new Game(repository, queue);

            var torpedo = new TestTorpedo("torp1", new NVector(0, 0), new NVector(1, 0));
            repository.Add("torp1", torpedo);

            game.Run(5);

            Assert.Equal(5, game.CurrentStep);
            Assert.Equal(new NVector(5, 0), torpedo.Position);
        }

        [Fact]
        public void GetSnapshot_ReturnsCurrentGameState()
        {
            var repository = new InMemoryGameObjectRepository();
            var queue = new SimpleCommandQueue();
            var game = new Game(repository, queue);

            var torpedo1 = new TestTorpedo("torp1", new NVector(10, 20), new NVector(1, 2));
            var torpedo2 = new TestTorpedo("torp2", new NVector(30, 40), new NVector(3, 4));

            repository.Add("torp1", torpedo1);
            repository.Add("torp2", torpedo2);

            game.Update();

            var snapshot = game.GetSnapshot();

            Assert.Equal(1, snapshot.Step);
            Assert.Equal(2, snapshot.Objects.Count);

            Assert.Contains(snapshot.Objects, o => o.Type == "TestTorpedo" &&
                                                    o.Position.Equals(new NVector(11, 22)) &&
                                                    o.Velocity.Equals(new NVector(1, 2)));
            Assert.Contains(snapshot.Objects, o => o.Type == "TestTorpedo" &&
                                                    o.Position.Equals(new NVector(33, 44)) &&
                                                    o.Velocity.Equals(new NVector(3, 4)));
        }

        [Fact]
        public void Stop_StopsInfiniteRun()
        {
            var repository = new InMemoryGameObjectRepository();
            var queue = new SimpleCommandQueue();
            var game = new Game(repository, queue);

            var torpedo = new TestTorpedo("torp1", new NVector(0, 0), new NVector(1, 0));
            repository.Add("torp1", torpedo);

            var thread = new System.Threading.Thread(() => game.Run());
            thread.Start();

            System.Threading.Thread.Sleep(100);
            game.Stop();
            thread.Join(1000);

            var stepsAfterStop = game.CurrentStep;
            System.Threading.Thread.Sleep(100);

            Assert.Equal(stepsAfterStop, game.CurrentStep);
        }
    }

    public class TestTorpedo : IMoving
    {
        public string Id { get; }
        public NVector Position { get; set; }
        public NVector Velocity { get; }

        public TestTorpedo(string id, NVector position, NVector velocity)
        {
            Id = id;
            Position = position;
            Velocity = velocity;
        }
    }

    public class BrokenTorpedo : IMoving
    {
        public string Id { get; }
        public NVector Position { get; set; }
        public NVector Velocity => throw new Exception("Velocity read error");

        public BrokenTorpedo(string id, NVector position)
        {
            Id = id;
            Position = position;
        }
    }

    public class TestCommand : ICommand
    {
        private readonly Action _action;
        public TestCommand(Action action) => _action = action;
        public void Execute() => _action();
    }

    public class ErrorCommand : ICommand
    {
        private readonly Exception _exception;
        public ErrorCommand(Exception exception) => _exception = exception;
        public void Execute() => throw _exception;
    }
}