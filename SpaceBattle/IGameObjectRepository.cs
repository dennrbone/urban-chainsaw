namespace SpaceBattle;

public interface IGameObjectRepository
{
    void Add(string id, object gameObject);
    void Remove(string id);
    object Get(string id);
    IEnumerable<object> GetAll();
}