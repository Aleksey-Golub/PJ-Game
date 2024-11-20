using Code.Infrastructure;
using Code.Services;
using UnityEngine;

public class FinalPrize : SimpleObject, ICreatedByIdGameObject
{
    [SerializeField] private SimpleObjectType _type;
    [SerializeField] private FinalPrizeView _view;
    private bool _playerInTrigger;

    protected override SimpleObjectType Type => _type;

    private void Start()
    {
        if (SceneBuiltInItem)
        {
            var audio = AllServices.Container.Single<IAudioService>();
            var gameFactory = AllServices.Container.Single<IGameFactory>();

            Construct(audio);
            gameFactory.RegisterProgressWatchersExternal(gameObject);
        }
    }

    internal void Construct(IAudioService audio)
    {
        _view.Construct(audio);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.TryGetComponent(out Player player))
            return;

        if (_playerInTrigger)
            return;

        _playerInTrigger = true;
        _view.ShowInteract();
        _view.PlayInteractSound();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.TryGetComponent(out Player player))
            return;

        if (!_playerInTrigger)
            return;

        _playerInTrigger = false;
        _view.HideInteract();
        _view.StopInteractSound();
    }

    void ICreatedByIdGameObject.Accept(ICreatedByIdGameObjectVisitor visitor) => visitor.Visit(this);
}
