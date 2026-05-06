namespace SpaceBattle;

public interface IMoving
{
    NVector Position { get; set; }
    NVector Velocity { get; }
}