using System.Collections.Generic;
using UnityEngine;

internal class ResourceFactory : MonoSingleton<ResourceFactory>
{
    [SerializeField] private Resource _resourcePrefab;
    [SerializeField, Min(1)] private int _poolSize = 1;

    private Queue<Resource> _pool;
    private List<Resource> _droppedResources;

    public IReadOnlyList<Resource> DroppedResources => _droppedResources;

    protected override void Awake()
    {
        base.Awake();

        _pool = new Queue<Resource>(_poolSize);
        _droppedResources = new List<Resource>();
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

        _droppedResources.Add(resource);
        return resource;
    }

    internal void Recycle(Resource resource)
    {
        resource.gameObject.SetActive(false);

        _droppedResources.Remove(resource);
        _pool.Enqueue(resource);
    }

    private void FillPool()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            var resource = Instantiate(_resourcePrefab, transform);
            resource.Construct(this);

            resource.gameObject.SetActive(false);

            _pool.Enqueue(resource);
        }
    }
}
