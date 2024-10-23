using UnityEngine;

internal class ToolFactory : MonoSingleton<ToolFactory>, IRecyclableFactory
{
    [SerializeField] private Tool _toolPrefab;
    [SerializeField, Min(1)] private int _poolSize = 1;

    private Pool<Tool> _pool;

    protected override void Awake()
    {
        base.Awake();

        var audio = AudioService.Instance;
        Construct(audio);
    }

    private void Construct(AudioService audio)
    {
        _pool = new Pool<Tool>(_toolPrefab, transform, _poolSize, this, audio);
    }

    internal Tool Get(Vector3 position, Quaternion rotation)
    {
        var tool = _pool.Get(position, rotation);
        //(tool as IPoolable).Construct(this);

        return tool;
    }

    public void Recycle(IPoolable tool)
    {
        _pool.Recycle(tool as Tool);
    }
}
