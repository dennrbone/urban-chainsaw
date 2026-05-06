using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceBattle;

public class CreateMacroCommandStrategy
{
    private readonly string _commandSpec;

    public CreateMacroCommandStrategy(string commandSpec) => _commandSpec = commandSpec;

    public ICommand Resolve(object[] args)
    {
        var commandNames = Ioc.Resolve<IEnumerable<string>>(_commandSpec);

        if (commandNames == null)
            throw new InvalidOperationException($"Spec '{_commandSpec}' not registered");

        var commands = commandNames
            .Select(name => Ioc.Resolve<ICommand>(name, args))
            .ToArray();

        if (commands.Any(c => c == null))
            throw new InvalidOperationException($"One or more commands not registered");

        return Ioc.Resolve<ICommand>("Commands.Macro", (object)commands);
    }
}