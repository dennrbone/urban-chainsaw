using System;
using System.Collections.Generic;

namespace SpaceBattle;

public class MacroCommand : ICommand
{
    private readonly IEnumerable<ICommand> _commands;

    public MacroCommand(IEnumerable<ICommand> commands) => _commands = commands;

    public void Execute()
    {
        _commands.ToList().ForEach(c => c.Execute());
    }
}