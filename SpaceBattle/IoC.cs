using System.Collections.Concurrent;

namespace SpaceBattle;

public static class Ioc
{
    private static readonly ConcurrentDictionary<string, Func<object[], object>> _strategies = new();

    static Ioc()
    {
        _strategies["IoC.Register"] = (args) => new RegisterCommand((string)args[0], (Func<object[], object>)args[1]);
    }

    public static T Resolve<T>(string key, params object[] args)
    {
        if (_strategies.TryGetValue(key, out var strategy))
        {
            return (T)strategy(args);
        }
        throw new InvalidOperationException($"Dependency '{key}' not registered");
    }

    private class RegisterCommand(string key, Func<object[], object> strategy) : ICommand
    {
        public void Execute() => _strategies[key] = strategy;
    }
}