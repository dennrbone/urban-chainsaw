namespace SpaceBattle
{
    public class Torpedo : IMoving
    {
        public string Id { get; }
        public NVector Position { get; set; }
        public NVector Velocity { get; }

        public Torpedo(string id, NVector position, NVector velocity)
        {
            Id = id;
            Position = position;
            Velocity = velocity;
        }
    }
}