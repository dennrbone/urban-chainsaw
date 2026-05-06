using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle;

public class MoveCommand(IMoving movingObject) : ICommand
{
    private readonly IMoving _movingObject = movingObject;

    public void Execute()
    {
        try
        {
            _movingObject.Position += _movingObject.Velocity;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to move the object", ex);
        }
    }
}