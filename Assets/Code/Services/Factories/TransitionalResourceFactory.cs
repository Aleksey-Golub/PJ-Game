using UnityEngine;

internal class TransitionalResourceFactory: MonoSingleton<TransitionalResourceFactory>, IRecyclableFactory
{
    [SerializeField] private TransitionalResource _TransitionalResourcePrefab;
    [SerializeField, Min(1)] private int _poolSize = 1;

    private Pool<TransitionalResource> _pool;

    protected override void Awake()
    {
        base.Awake();

        var audio = AudioService.Instance;
        Construct(audio);
    }

    private void Construct(AudioService audio)
    {
        _pool = new Pool<TransitionalResource>(_TransitionalResourcePrefab, transform, _poolSize, this, audio);
    }

    internal TransitionalResource Get(Vector3 position, Quaternion rotation)
    {
        var popup = _pool.Get(position, rotation);
        //popup.Construct(this);

        return popup;
    }

    public void Recycle(IPoolable popup)
    {
        _pool.Recycle(popup as TransitionalResource);
    }
}
