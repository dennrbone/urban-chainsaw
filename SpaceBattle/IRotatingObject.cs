namespace SpaceBattle;

public interface IRotatingObject
{
    Angle Angle { get; set; }
    Angle AngularVelocity { get; }
}
