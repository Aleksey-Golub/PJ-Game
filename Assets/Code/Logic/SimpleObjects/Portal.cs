using Code.Infrastructure;
using Code.Services;
using UnityEngine;

public class Portal : SimpleObjectBase, ICreatedByIdGameObject
{
    [SerializeField] private Transform _teleportPoint;
    [SerializeField] private SimpleObjectType _portalType;

    [Header("View")]
    [SerializeField] private AudioClip _teleportClip;

    private IAudioService _audio;

    protected override SimpleObjectType Type => _portalType;

    private void Start()
    {
        if (SceneBuiltInItem)
        {
            var gameFactory = AllServices.Container.Single<IGameFactory>();
            var audio = AllServices.Container.Single<IAudioService>();

            Construct(audio);
            gameFactory.RegisterProgressWatchersExternal(gameObject);
        }
    }

    internal void Construct(IAudioService audio)
    {
        _audio = audio;
    }

    internal void Interact(Player player)
    {
        PlayTeleportSound();
        TeleportPlayer(player);
    }

    private void TeleportPlayer(Player player)
    {
        player.Teleport(_teleportPoint.position);
    }

    private void PlayTeleportSound()
    {
        if (_teleportClip != null)
            _audio.PlaySfxAtPosition(_teleportClip, transform.position);
    }

    void ICreatedByIdGameObject.Accept(ICreatedByIdGameObjectVisitor visitor) => visitor.Visit(this);
}
