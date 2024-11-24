using Code.Data;
using Code.Infrastructure;
using Code.Services;
using UnityEngine;

public class TutorialOnly : SimpleObjectBase, ICreatedByIdGameObject
{
    [GameObjectIdHolder]
    [SerializeField] private string _gameObjectId;

    protected override SimpleObjectType Type => SimpleObjectType.TutorialOnly;

    public string GameObjectId => _gameObjectId;

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

    public override void ReadProgress(GameProgress progress)
    {
        var tutorialObjectsOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].TutorialProgress.TutorialObjectsDatas.TutorialObjectsOnScene;

        // we are build-in-scene object and it is first start of level
        if (!tutorialObjectsOnScene.Dictionary.TryGetValue(Id, out var myState))
            return;

        // restore state
        transform.position = myState.Position.AsUnityVector();
    }

    public override void WriteToProgress(GameProgress progress)
    {
        var tutorialObjectsOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].TutorialProgress.TutorialObjectsDatas.TutorialObjectsOnScene;

        // just to optimize
        tutorialObjectsOnScene.Dictionary.TryGetValue(Id, out var data);
        if (data != null && !HasChangesBetweenSavedStateAndCurrentState(data))
            return;

        tutorialObjectsOnScene.Dictionary[Id] = new TutorialObjectOnSceneData(
            transform.position.AsVectorData(),
            Type,
            SceneBuiltInItem,

            _gameObjectId
            );
    }

    void ICreatedByIdGameObject.Accept(ICreatedByIdGameObjectVisitor visitor) => visitor.Visit(this);
}
