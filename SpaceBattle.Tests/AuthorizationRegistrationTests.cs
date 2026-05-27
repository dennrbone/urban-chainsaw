using System;
using System.Collections.Generic;
using Moq;
using Xunit;
using SpaceBattle;

namespace SpaceBattleTests
{
    public class AuthorizationRegistrationTests
    {
        [Fact]
        public void IoC_AuthorizationRegistration_ResolvesWithoutErrors()
        {
            var testPermissions = new List<(string PlayerId, string Action)> 
            { 
                ("player123", "Fire") 
            };

            new RegisterIoCDependencyAuthorization().Execute();

            var authService = Ioc.Resolve<IAuthorizationService>("Services.Authorization", testPermissions);

            Assert.NotNull(authService);
            Assert.IsType<AuthorizationService>(authService);
        }
    }
}