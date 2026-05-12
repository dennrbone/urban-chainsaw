using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SpaceBattle.Repositories;

public class InMemoryGameObjectRepository : IGameObjectRepository
{
    private readonly ConcurrentDictionary<string, object> _storage = new();

    public void Add(string id, object gameObject)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Идентификатор не может быть пустым.", nameof(id));

        if (gameObject == null)
            throw new ArgumentNullException(nameof(gameObject), "Объект не может быть null.");

        if (!_storage.TryAdd(id, gameObject))
        {
            throw new InvalidOperationException($"Объект с ID '{id}' уже существует.");
        }
    }

    public void Remove(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Идентификатор не может быть пустым.", nameof(id));

        if (!_storage.TryRemove(id, out _))
        {
            throw new KeyNotFoundException($"Объект с ID '{id}' не найден для удаления.");
        }
    }

    public object Get(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Идентификатор не может быть пустым.", nameof(id));

        if (_storage.TryGetValue(id, out var gameObject))
        {
            return gameObject;
        }

        throw new KeyNotFoundException($"Объект с ID '{id}' не найден.");
    }

    public IEnumerable<object> GetAll()
    {
        return _storage.Values;
    }
}