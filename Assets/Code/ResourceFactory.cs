using System.Collections.Generic;
using UnityEngine;

internal class ResourceFactory : MonoSingleton<ResourceFactory>
{
    [SerializeField] private Resource _resourcePrefab;
    [SerializeField, Min(1)] private int _poolSize = 1;

    private Queue<Resource> _pool;

    protected override void Awake()
    {
        base.Awake();

        FillPool();
    }

    internal Resource Get(Vector3 position, Quaternion rotation)
    {
        if (_pool.TryDequeue(out var resource))
        {
            resource.gameObject.SetActive(true);
        }
        else
        {
            resource = Instantiate(_resourcePrefab, transform);
            resource.Construct(this);
        }

        resource.transform.SetPositionAndRotation(position, rotation);
        return resource;
    }

    private void FillPool()
    {
        _pool = new Queue<Resource>(_poolSize);

        for (int i = 0; i < _poolSize; i++)
        {
            var resource = Instantiate(_resourcePrefab, transform);
            resource.Construct(this);

            resource.gameObject.SetActive(false);

            _pool.Enqueue(resource);
        }
    }
}

internal abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    internal static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = (T)this;
        }
        else
        {
            Destroy(this);
        }
    }
    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
