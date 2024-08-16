using UnityEngine;

internal class Tool : MonoBehaviour, IPoolable
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private ToolView _view;

    private Dropper _dropper;
    private IRecyclableFactory _factory;
    private ToolConfig _config;

    internal ToolType Type => _config.Type;

    void IPoolable.Construct(IRecyclableFactory factory)
    {
        _factory = factory;
        _dropper = new();
    }

    internal void Init(ToolConfig config)
    {
        _config = config;

        _view.Init(_config.Sprite);
        _collider.enabled = false;
    }

    internal void MoveAfterDrop(DropData dropData)
    {
        _dropper.MoveAfterDrop(this, _view, _collider, dropData);
    }

    internal void Collect()
    {
        AudioSource.PlayClipAtPoint(_config.PickupAudio, transform.position);

        _factory.Recycle(this);
    }
}

