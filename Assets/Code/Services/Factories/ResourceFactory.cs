using System.Collections.Generic;
using UnityEngine;

internal class ResourceFactory : MonoSingleton<ResourceFactory>, IRecyclableFactory
{
    [SerializeField] private Resource _resourcePrefab;
    [SerializeField, Min(1)] private int _poolSize = 1;

    private Pool<Resource> _pool;
    private List<Resource> _droppedResources;

    public IReadOnlyList<Resource> DroppedResources => _droppedResources;

    protected override void Awake()
    {
        base.Awake();

        var audio = AudioService.Instance;
        Construct(audio);
    }

    private void Construct(AudioService audio)
    {
        _pool = new Pool<Resource>(_resourcePrefab, transform, _poolSize, this, audio);
        _droppedResources = new List<Resource>();
    }

    internal Resource Get(Vector3 position, Quaternion rotation)
    {
        var res = _pool.Get(position, rotation);
        //res.Construct(this);

        _droppedResources.Add(res);

        return res;
    }

    public void Recycle(IPoolable resource)
    {
        _droppedResources.Remove(resource as Resource);

        _pool.Recycle(resource as Resource);
    }
}
