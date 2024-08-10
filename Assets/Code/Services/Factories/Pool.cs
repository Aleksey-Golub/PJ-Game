using System.Collections.Generic;
using UnityEngine;

internal class Pool<T> where T : MonoBehaviour
{
    private readonly Queue<T> _pool;
    private readonly T _prefab;
    private readonly Transform _parent;

    public Pool(T prefab, Transform parent, int initialPoolSize)
    {
        _prefab = prefab;
        _parent = parent;

        _pool = new Queue<T>(initialPoolSize);
        FillPool(initialPoolSize);
    }
    internal T Get(Vector3 position, Quaternion rotation)
    {
        if (_pool.TryDequeue(out var obj))
        {
            obj.gameObject.SetActive(true);
        }
        else
        {
            obj = UnityEngine.Object.Instantiate(_prefab, _parent);
        }

        obj.transform.SetPositionAndRotation(position, rotation);

        return obj;
    }

    internal void Recycle(T go)
    {
        go.gameObject.SetActive(false);

        _pool.Enqueue(go);
    }

    private void FillPool(int initialPoolSize)
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            var newGO = UnityEngine.Object.Instantiate(_prefab, _parent);

            newGO.gameObject.SetActive(false);

            _pool.Enqueue(newGO);
        }
    }
}
