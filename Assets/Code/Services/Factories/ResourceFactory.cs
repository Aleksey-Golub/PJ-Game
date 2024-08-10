using System.Collections.Generic;
using UnityEngine;

internal class ResourceFactory : MonoSingleton<ResourceFactory>
{
    [SerializeField] private Resource _resourcePrefab;
    [SerializeField, Min(1)] private int _poolSize = 1;

    private Pool<Resource> _pool;
    private List<Resource> _droppedResources;

    public IReadOnlyList<Resource> DroppedResources => _droppedResources;

    protected override void Awake()
    {
        base.Awake();

        _pool = new Pool<Resource>(_resourcePrefab, transform, _poolSize);
        _droppedResources = new List<Resource>();
    }

    internal Resource Get(Vector3 position, Quaternion rotation)
    {
        var res = _pool.Get(position, rotation);
        res.Construct(this);

        _droppedResources.Add(res);

        return res;
    }

    internal void Recycle(Resource resource)
    {
        _droppedResources.Remove(resource);

        _pool.Recycle(resource);
    }
}
