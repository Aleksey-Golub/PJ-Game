using Code.Infrastructure;
using Code.Services;
using UnityEngine;

/// <summary>
/// Clear Simple object: bridge, wall, debris etc.
/// </summary>
public class SimpleObject : SimpleObjectBase, ICreatedByIdGameObject
{
    [SerializeField] private SimpleObjectType _type;

    protected override SimpleObjectType Type => _type;

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

    void ICreatedByIdGameObject.Accept(ICreatedByIdGameObjectVisitor visitor) => visitor.Visit(this);
}
