using Code.Infrastructure;
using Code.Services;
using UnityEngine;

public class Portal : SimpleObjectBase, ICreatedByIdGameObject
{
    [SerializeField] private Transform _teleportPoint;
    [SerializeField] private SimpleObjectType _portalType;

    protected override SimpleObjectType Type => _portalType;

    private void Start()
    {
        if (SceneBuiltInItem)
        {
            var gameFactory = AllServices.Container.Single<IGameFactory>();

            Construct();
            gameFactory.RegisterProgressWatchersExternal(gameObject);
        }
    }

    internal void Construct()
    {

    }

    internal void Interact(Player player)
    {
        TeleportPlayer(player);
    }

    private void TeleportPlayer(Player player)
    {
        player.Teleport(_teleportPoint.position);
    }

    void ICreatedByIdGameObject.Accept(ICreatedByIdGameObjectVisitor visitor) => visitor.Visit(this);
}
