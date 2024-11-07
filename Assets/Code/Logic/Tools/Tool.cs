using Code.Services;
using UnityEngine;

public class Tool : MonoBehaviour, IPoolable
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private ToolView _view;

    private Dropper _dropper;
    private IRecyclableFactory _factory;
    private ToolConfig _config;
    private IAudioService _audio;

    public bool IsConstructed { get; private set; }
    internal ToolType Type => _config.Type;

    public void Construct(IRecyclableFactory factory, IAudioService audio)
    {
        _factory = factory;
        _audio = audio;

        _dropper = new();

        IsConstructed = true;
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
        _audio.PlaySfxAtPosition(_config.PickupAudio, transform.position);

        _factory.Recycle(this);
    }
}

