using System;
using System.Collections.Generic;

namespace SpaceBattle;

public class RegisterIoCDependencyAuthorization : ICommand
{
    private static IAuthorizationService? _instance;

    public void Execute()
    {
        Ioc.Resolve<ICommand>("IoC.Register", "Services.Authorization", (Func<object[], object>)((args) => 
        {
            if (args != null && args.Length > 0)
            {
                var permissions = (IEnumerable<(string PlayerId, string Action)>)args[0];
                _instance = new AuthorizationService(permissions);
            }
            
            return _instance ?? throw new InvalidOperationException("Authorization service is not initialized.");
        })).Execute();
    }
}