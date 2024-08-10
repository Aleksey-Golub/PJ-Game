using UnityEngine;

internal class Tool : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private ToolView _view;
    [SerializeField] private Dropper _dropper;

    private ToolsFactory _factory;
    private ToolConfig _config;

    internal ToolType Type => _config.Type;

    internal void Construct(ToolsFactory factory)
    {
        _factory = factory;
    }

    internal void Init(ToolConfig config)
    {
        _config = config;

        _view.Init(_config.Sprite);
        _collider.enabled = false;
    }

    internal void MoveAfterDrop()
    {
        _dropper.MoveAfterDrop(this, _view, _collider);
    }

    internal void Collect()
    {
        AudioSource.PlayClipAtPoint(_config.PickupAudio, transform.position);

        _factory.Recycle(this);
    }
}
