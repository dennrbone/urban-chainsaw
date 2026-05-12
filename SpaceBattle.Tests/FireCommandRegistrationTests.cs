using System;
using System.Collections.Generic;
using Moq;
using Xunit;
using SpaceBattle;

namespace SpaceBattleTests
{
    public class FireCommandRegistrationTests
    {
        [Fact]
        public void IoC_FireCommandRegistration_ResolvesWithoutErrors()
        {
            // Arrange: Регистрируем в Ioc заглушки, чтобы фабрика команды не упала при сборке
            var repoMock = new Mock<IGameObjectRepository>();
            repoMock.Setup(r => r.Get("ship123")).Returns(new Dictionary<string, object>());

            Ioc.Resolve<ICommand>("IoC.Register", "Repositories.GameObject", (Func<object[], object>)(args => repoMock.Object)).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Game.Queue", (Func<object[], object>)(args => (Action<ICommand>)(cmd => { }))).Execute();
            Ioc.Resolve<ICommand>("IoC.Register", "Adapters.IMoving", (Func<object[], object>)(args => new Mock<IMoving>().Object)).Execute();

            // Act: Запускаем твою команду регистрации зависимости
            new RegisterIoCDependencyFireCommand().Execute();

            // Извлекаем команду из контейнера, как это делает игровой цикл
            var command = Ioc.Resolve<ICommand>("Commands.Fire", "ship123", new NVector(1, 0), "player1", 5.0);

            // Assert: Проверяем, что из Ioc вылетел именно твой класс FireCommand
            Assert.IsType<FireCommand>(command);
        }
    }
}
