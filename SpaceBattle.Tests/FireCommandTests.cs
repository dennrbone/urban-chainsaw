using System;
using System.Collections.Generic;
using Moq;
using Xunit;
using SpaceBattle;

namespace SpaceBattleTests
{
    public class FireCommandTests
    {
        [Fact]
        public void Execute_SuccessfulFire_CalculatesVelocityAndEnqueuesMove()
        {
            // Arrange
            var shipMock = new Mock<IMoving>();
            var initialPosition = new NVector(10, 10);
            var initialVelocity = new NVector(1, 1);
            shipMock.SetupGet(s => s.Position).Returns(initialPosition);
            shipMock.SetupGet(s => s.Velocity).Returns(initialVelocity);

            var repoMock = new Mock<IGameObjectRepository>();
            var commandQueue = new List<ICommand>();
            Action<ICommand> queueAction = cmd => commandQueue.Add(cmd);
            var moveCommandMock = new Mock<ICommand>();

            // Задаем ожидаемую итоговую скорость торпеды
            var expectedVelocity = new NVector(7, 9);

            // Чисто и изолированно регистрируем заглушки в Ioc.
            // Вместо вызова конфликтующего класса Vector, возвращаем готовые экземпляры NVector.
            Ioc.Resolve<ICommand>("IoC.Register", "Authorization.Check", (Func<object[], object>)(args => true)).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Normalize", (Func<object[], object>)(args => new NVector(0, 0))).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Multiply", (Func<object[], object>)(args => new NVector(0, 0))).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Vector.Add", (Func<object[], object>)(args => expectedVelocity)).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Commands.Move", (Func<object[], object>)(args => moveCommandMock.Object)).Execute();

            var direction = new NVector(3, 4);
            var command = new FireCommand(shipMock.Object, direction, repoMock.Object, "player1", 10.0, queueAction);

            // Act
            command.Execute();

            // Assert
            repoMock.Verify(r => r.Add(It.IsAny<string>(), It.Is<IDictionary<string, object>>(dict =>
                dict["Position"].Equals(initialPosition) &&
                dict["Velocity"].Equals(expectedVelocity)
            )), Times.Once);

            Assert.Single(commandQueue);
        }

        [Fact]
        public void Execute_UserNotAuthorized_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var shipMock = new Mock<IMoving>();
            Ioc.Resolve<ICommand>("IoC.Register", "Authorization.Check", (Func<object[], object>)(args => false)).Execute();

            var command = new FireCommand(shipMock.Object, new NVector(1, 0), new Mock<IGameObjectRepository>().Object, "bad_user", 5, cmd => { });

            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() => command.Execute());
        }

        [Fact]
        public void Constructor_WithNullShip_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new FireCommand(null!, new NVector(1, 0), new Mock<IGameObjectRepository>().Object, "user", 5, cmd => { }));
        }

        [Fact]
        public void Constructor_WithNullDirection_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new FireCommand(new Mock<IMoving>().Object, null!, new Mock<IGameObjectRepository>().Object, "user", 5, cmd => { }));
        }

        [Fact]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new FireCommand(new Mock<IMoving>().Object, new NVector(1, 0), null!, "user", 5, cmd => { }));
        }

        [Fact]
        public void Constructor_WithNullUserId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new FireCommand(new Mock<IMoving>().Object, new NVector(1, 0), new Mock<IGameObjectRepository>().Object, null!, 5, cmd => { }));
        }

        [Fact]
        public void Constructor_WithNullQueue_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new FireCommand(new Mock<IMoving>().Object, new NVector(1, 0), new Mock<IGameObjectRepository>().Object, "user", 5, null!));
        }
    }
}
