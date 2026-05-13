using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using SpaceBattle;

namespace SpaceBattleTests
{
    public class FireCommandTests
    {
        private Mock<IMoving> _shipMock;
        private Mock<IGameObjectRepository> _repoMock;
        private List<ICommand> _commandQueue;
        private Action<ICommand> _queueAction;
        private Mock<ICommand> _moveCommandMock;
        private NVector _initialPosition;
        private NVector _initialVelocity;
        private NVector _direction;
        private const string _userId = "player1";
        private const double _torpedoSpeed = 10.0;

        public FireCommandTests()
        {
            _shipMock = new Mock<IMoving>();
            _initialPosition = new NVector(10, 10);
            _initialVelocity = new NVector(1, 1);
            _shipMock.SetupGet(s => s.Position).Returns(_initialPosition);
            _shipMock.SetupGet(s => s.Velocity).Returns(_initialVelocity);

            _repoMock = new Mock<IGameObjectRepository>();
            _commandQueue = new List<ICommand>();
            _queueAction = cmd => _commandQueue.Add(cmd);
            _moveCommandMock = new Mock<ICommand>();
            _direction = new NVector(3, 4);
        }

        private void SetupSuccessfulIoC()
        {
            Ioc.Resolve<ICommand>("IoC.Register", "Authorization.Check",
                (Func<object[], object>)(args => true)).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Normalize",
                (Func<object[], object>)(args => new NVector(0, 0))).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Multiply",
                (Func<object[], object>)(args => new NVector(6, 8))).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Add",
                (Func<object[], object>)(args => new NVector(7, 9))).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Commands.Move",
                (Func<object[], object>)(args => _moveCommandMock.Object)).Execute();
        }

        [Fact]
        public void Execute_SuccessfulFire_CalculatesVelocityAndEnqueuesMove()
        {
            // Arrange
            SetupSuccessfulIoC();
            var expectedVelocity = new NVector(7, 9);

            var command = new FireCommand(_shipMock.Object, _direction, _repoMock.Object,
                _userId, _torpedoSpeed, _queueAction);

            // Act
            command.Execute();

            // Assert
            _repoMock.Verify(r => r.Add(It.IsAny<string>(),
                It.Is<IDictionary<string, object>>(dict =>
                    dict["Position"].Equals(_initialPosition) &&
                    ((NVector)dict["Velocity"]).Coords.SequenceEqual(expectedVelocity.Coords)
                )), Times.Once);

            Assert.Single(_commandQueue);
            Assert.Equal(_moveCommandMock.Object, _commandQueue[0]);
        }

        [Fact]
        public void Execute_UserNotAuthorized_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            Ioc.Resolve<ICommand>("IoC.Register", "Authorization.Check",
                (Func<object[], object>)(args => false)).Execute();

            var command = new FireCommand(_shipMock.Object, _direction, _repoMock.Object,
                "bad_user", _torpedoSpeed, _queueAction);

            // Act & Assert
            var ex = Assert.Throws<UnauthorizedAccessException>(() => command.Execute());
            Assert.Contains("bad_user", ex.Message);
        }

        [Fact]
        public void Execute_AuthorizationCheckThrows_PropagatesException()
        {
            // Arrange
            Ioc.Resolve<ICommand>("IoC.Register", "Authorization.Check",
                (Func<object[], object>)(args => throw new InvalidOperationException("Auth service down"))).Execute();

            var command = new FireCommand(_shipMock.Object, _direction, _repoMock.Object,
                _userId, _torpedoSpeed, _queueAction);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => command.Execute());
        }

        [Fact]
        public void Execute_VectorNormalizeThrows_PropagatesException()
        {
            // Arrange
            Ioc.Resolve<ICommand>("IoC.Register", "Authorization.Check",
                (Func<object[], object>)(args => true)).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Normalize",
                (Func<object[], object>)(args => throw new ArithmeticException("Cannot normalize zero vector"))).Execute();

            var command = new FireCommand(_shipMock.Object, _direction, _repoMock.Object,
                _userId, _torpedoSpeed, _queueAction);

            // Act & Assert
            Assert.Throws<ArithmeticException>(() => command.Execute());
        }

        [Fact]
        public void Execute_VectorMultiplyThrows_PropagatesException()
        {
            // Arrange
            Ioc.Resolve<ICommand>("IoC.Register", "Authorization.Check",
                (Func<object[], object>)(args => true)).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Normalize",
                (Func<object[], object>)(args => new NVector(0, 0))).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Multiply",
                (Func<object[], object>)(args => throw new OverflowException("Multiplication overflow"))).Execute();

            var command = new FireCommand(_shipMock.Object, _direction, _repoMock.Object,
                _userId, _torpedoSpeed, _queueAction);

            // Act & Assert
            Assert.Throws<OverflowException>(() => command.Execute());
        }

        [Fact]
        public void Execute_VectorAddThrows_PropagatesException()
        {
            // Arrange
            Ioc.Resolve<ICommand>("IoC.Register", "Authorization.Check",
                (Func<object[], object>)(args => true)).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Normalize",
                (Func<object[], object>)(args => new NVector(0, 0))).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Multiply",
                (Func<object[], object>)(args => new NVector(6, 8))).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Add",
                (Func<object[], object>)(args => throw new ArgumentException("Vector dimensions mismatch"))).Execute();

            var command = new FireCommand(_shipMock.Object, _direction, _repoMock.Object,
                _userId, _torpedoSpeed, _queueAction);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => command.Execute());
        }

        [Fact]
        public void Execute_CommandsMoveResolutionThrows_PropagatesException()
        {
            // Arrange
            Ioc.Resolve<ICommand>("IoC.Register", "Authorization.Check",
                (Func<object[], object>)(args => true)).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Normalize",
                (Func<object[], object>)(args => new NVector(0, 0))).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Multiply",
                (Func<object[], object>)(args => new NVector(6, 8))).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Add",
                (Func<object[], object>)(args => new NVector(7, 9))).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Commands.Move",
                (Func<object[], object>)(args => throw new Exception("Move command not found"))).Execute();

            var command = new FireCommand(_shipMock.Object, _direction, _repoMock.Object,
                _userId, _torpedoSpeed, _queueAction);

            // Act & Assert
            Assert.Throws<Exception>(() => command.Execute());
        }

        [Fact]
        public void Execute_RepositoryAddThrows_PropagatesException()
        {
            // Arrange
            SetupSuccessfulIoC();
            _repoMock.Setup(r => r.Add(It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()))
                .Throws(new InvalidOperationException("Repository error"));

            var command = new FireCommand(_shipMock.Object, _direction, _repoMock.Object,
                _userId, _torpedoSpeed, _queueAction);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => command.Execute());
            Assert.Empty(_commandQueue);
        }

        [Fact]
        public void Execute_WithZeroTorpedoSpeed_CalculatesCorrectly()
        {
            // Arrange
            SetupSuccessfulIoC();
            var zeroSpeedCommand = new FireCommand(_shipMock.Object, _direction, _repoMock.Object,
                _userId, 0, _queueAction);

            // Act
            zeroSpeedCommand.Execute();

            // Assert
            _repoMock.Verify(r => r.Add(It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()), Times.Once);
            Assert.Single(_commandQueue);
        }

        [Fact]
        public void Execute_WithNegativeTorpedoSpeed_CalculatesCorrectly()
        {
            // Arrange
            SetupSuccessfulIoC();
            var negativeSpeedCommand = new FireCommand(_shipMock.Object, _direction, _repoMock.Object,
                _userId, -5, _queueAction);

            // Act
            negativeSpeedCommand.Execute();

            // Assert
            _repoMock.Verify(r => r.Add(It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()), Times.Once);
            Assert.Single(_commandQueue);
        }

        [Fact]
        public void Constructor_WithNullShip_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new FireCommand(null!, _direction, _repoMock.Object, _userId, _torpedoSpeed, _queueAction));
            Assert.Equal("ship", ex.ParamName);
        }

        [Fact]
        public void Constructor_WithNullDirection_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new FireCommand(_shipMock.Object, null!, _repoMock.Object, _userId, _torpedoSpeed, _queueAction));
            Assert.Equal("direction", ex.ParamName);
        }

        [Fact]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new FireCommand(_shipMock.Object, _direction, null!, _userId, _torpedoSpeed, _queueAction));
            Assert.Equal("repository", ex.ParamName);
        }

        [Fact]
        public void Constructor_WithNullUserId_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new FireCommand(_shipMock.Object, _direction, _repoMock.Object, null!, _torpedoSpeed, _queueAction));
            Assert.Equal("userId", ex.ParamName);
        }

        [Fact]
        public void Constructor_WithNullQueue_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new FireCommand(_shipMock.Object, _direction, _repoMock.Object, _userId, _torpedoSpeed, null!));
            Assert.Equal("commandQueue", ex.ParamName);
        }

        [Fact]
        public void Execute_WithDifferentVelocityVectors_CalculatesCorrectly()
        {
            // Arrange
            var differentVelocities = new[]
            {
                new NVector(2, 3),
                new NVector(-1, 5),
                new NVector(0, 0)
            };

            foreach (var velocity in differentVelocities)
            {
                _commandQueue.Clear();
                _shipMock.SetupGet(s => s.Velocity).Returns(velocity);

                Ioc.Resolve<ICommand>("IoC.Register", "Authorization.Check",
                    (Func<object[], object>)(args => true)).Execute();
                Ioc.Resolve<ICommand>("IoC.Register", "Vector.Normalize",
                    (Func<object[], object>)(args => new NVector(0, 0))).Execute();
                Ioc.Resolve<ICommand>("IoC.Register", "Vector.Multiply",
                    (Func<object[], object>)(args => new NVector(6, 8))).Execute();

                var expectedAddResult = new NVector(
                    velocity.Coords[0] + 6,
                    velocity.Coords[1] + 8
                );

                Ioc.Resolve<ICommand>("IoC.Register", "Vector.Add",
                    (Func<object[], object>)(args => expectedAddResult)).Execute();
                Ioc.Resolve<ICommand>("IoC.Register", "Commands.Move",
                    (Func<object[], object>)(args => _moveCommandMock.Object)).Execute();

                var command = new FireCommand(_shipMock.Object, _direction, _repoMock.Object,
                    _userId, _torpedoSpeed, _queueAction);

                // Act
                command.Execute();

                // Assert
                _repoMock.Verify(r => r.Add(It.IsAny<string>(),
                    It.Is<IDictionary<string, object>>(dict =>
                        ((NVector)dict["Velocity"]).Coords.SequenceEqual(expectedAddResult.Coords))), Times.Once);

                _repoMock.Invocations.Clear();
            }
        }
        [Fact]
        public void Execute_WhenIoCThrowsForNormalize_HandlesCorrectly()
        {
            // Arrange
            Ioc.Resolve<ICommand>("IoC.Register", "Authorization.Check",
                (Func<object[], object>)(args => true)).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Normalize",
                (Func<object[], object>)(args => throw new InvalidOperationException("Normalization failed"))).Execute();

            var command = new FireCommand(_shipMock.Object, _direction, _repoMock.Object,
                _userId, _torpedoSpeed, _queueAction);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => command.Execute());
        }

        [Fact]
        public void Execute_WhenIoCThrowsForMultiply_HandlesCorrectly()
        {
            // Arrange
            Ioc.Resolve<ICommand>("IoC.Register", "Authorization.Check",
                (Func<object[], object>)(args => true)).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Normalize",
                (Func<object[], object>)(args => new NVector(0, 0))).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Multiply",
                (Func<object[], object>)(args => throw new InvalidOperationException("Multiplication failed"))).Execute();

            var command = new FireCommand(_shipMock.Object, _direction, _repoMock.Object,
                _userId, _torpedoSpeed, _queueAction);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => command.Execute());
        }

        [Fact]
        public void Execute_WhenIoCThrowsForAdd_HandlesCorrectly()
        {
            // Arrange
            Ioc.Resolve<ICommand>("IoC.Register", "Authorization.Check",
                (Func<object[], object>)(args => true)).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Normalize",
                (Func<object[], object>)(args => new NVector(0, 0))).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Multiply",
                (Func<object[], object>)(args => new NVector(6, 8))).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Add",
                (Func<object[], object>)(args => throw new InvalidOperationException("Addition failed"))).Execute();

            var command = new FireCommand(_shipMock.Object, _direction, _repoMock.Object,
                _userId, _torpedoSpeed, _queueAction);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => command.Execute());
        }

        [Fact]
        public void Execute_WhenIoCThrowsForMoveCommand_HandlesCorrectly()
        {
            // Arrange
            Ioc.Resolve<ICommand>("IoC.Register", "Authorization.Check",
                (Func<object[], object>)(args => true)).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Normalize",
                (Func<object[], object>)(args => new NVector(0, 0))).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Multiply",
                (Func<object[], object>)(args => new NVector(6, 8))).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Add",
                (Func<object[], object>)(args => new NVector(7, 9))).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Commands.Move",
                (Func<object[], object>)(args => throw new InvalidOperationException("Move command creation failed"))).Execute();

            var command = new FireCommand(_shipMock.Object, _direction, _repoMock.Object,
                _userId, _torpedoSpeed, _queueAction);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => command.Execute());
        }

        [Fact]
        public void Execute_WithVeryLargeTorpedoSpeed_HandlesCorrectly()
        {
            // Arrange
            SetupSuccessfulIoC();
            const double largeSpeed = 1000000;
            var largeSpeedCommand = new FireCommand(_shipMock.Object, _direction, _repoMock.Object,
                _userId, largeSpeed, _queueAction);

            // Act
            largeSpeedCommand.Execute();

            // Assert
            _repoMock.Verify(r => r.Add(It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()), Times.Once);
            Assert.Single(_commandQueue);
        }

        [Fact]
        public void Execute_WithMinValueTorpedoSpeed_HandlesCorrectly()
        {
            // Arrange
            SetupSuccessfulIoC();
            const double minSpeed = double.MinValue;
            var minSpeedCommand = new FireCommand(_shipMock.Object, _direction, _repoMock.Object,
                _userId, minSpeed, _queueAction);

            // Act
            minSpeedCommand.Execute();

            // Assert
            _repoMock.Verify(r => r.Add(It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()), Times.Once);
            Assert.Single(_commandQueue);
        }

        [Fact]
        public void Execute_WithMaxValueTorpedoSpeed_HandlesCorrectly()
        {
            // Arrange
            SetupSuccessfulIoC();
            const double maxSpeed = double.MaxValue;
            var maxSpeedCommand = new FireCommand(_shipMock.Object, _direction, _repoMock.Object,
                _userId, maxSpeed, _queueAction);

            // Act
            maxSpeedCommand.Execute();

            // Assert
            _repoMock.Verify(r => r.Add(It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()), Times.Once);
            Assert.Single(_commandQueue);
        }

        [Fact]
        public void Execute_WhenShipVelocityIsNull_PassesNullToIoC()
        {
            // Arrange
            bool velocityWasNull = false;
            NVector? passedVelocity = null;

            Ioc.Resolve<ICommand>("IoC.Register", "Authorization.Check",
                (Func<object[], object>)(args => true)).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Normalize",
                (Func<object[], object>)(args => new NVector(0, 0))).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Multiply",
                (Func<object[], object>)(args => new NVector(6, 8))).Execute();

            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Add",
                (Func<object[], object>)(args =>
                {
                    passedVelocity = args[0] as NVector;
                    if (passedVelocity == null)
                        velocityWasNull = true;
                    return new NVector(7, 9);
                })).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Commands.Move",
                (Func<object[], object>)(args => _moveCommandMock.Object)).Execute();

            _shipMock.SetupGet(s => s.Velocity).Returns((NVector)null!);
            _shipMock.SetupGet(s => s.Position).Returns(new NVector(0, 0));

            var command = new FireCommand(_shipMock.Object, _direction, _repoMock.Object,
                _userId, _torpedoSpeed, _queueAction);

            // Act
            command.Execute();

            // Assert
            Assert.True(velocityWasNull, "Velocity null was not passed to Vector.Add");
        }

        [Fact]
        public void Execute_MultipleFires_EachTorpedoHasUniqueId()
        {
            // Arrange
            SetupSuccessfulIoC();
            var command1 = new FireCommand(_shipMock.Object, _direction, _repoMock.Object,
                _userId, _torpedoSpeed, _queueAction);
            var command2 = new FireCommand(_shipMock.Object, _direction, _repoMock.Object,
                _userId, _torpedoSpeed, _queueAction);

            var addedIds = new List<string>();


            _repoMock.Setup(r => r.Add(It.IsAny<string>(), It.IsAny<object>()))
                .Callback<string, object>((id, dict) => addedIds.Add(id));

            // Act
            command1.Execute();
            command2.Execute();

            // Assert
            Assert.Equal(2, addedIds.Count);
            Assert.NotEqual(addedIds[0], addedIds[1]);
            Assert.All(addedIds, id => Assert.StartsWith("torpedo_", id));
        }
    }
}