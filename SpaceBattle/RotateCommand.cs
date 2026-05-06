namespace SpaceBattle;

public class RotateCommand : ICommand
{
    private readonly IRotatingObject _rotatingObject;

    public RotateCommand(IRotatingObject rotatingObject)
    {
        _rotatingObject = rotatingObject;
    }

    public void Execute()
    {
        _rotatingObject.Angle = _rotatingObject.Angle + _rotatingObject.AngularVelocity;
    }
}
