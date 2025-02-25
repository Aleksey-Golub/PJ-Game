﻿using Code.Services;
using UnityEngine;
using Code.Data;
using Code.Infrastructure;

[SelectionBase]
public abstract class SimpleObjectBase : MonoBehaviour, ISavedProgressReader, ISavedProgressWriter, IUniqueIdHolder, IPossibleSceneBuiltInItem
{
    [field: SerializeField] public bool SceneBuiltInItem { get; private set; }
    [field: SerializeField] public UniqueId UniqueId { get; private set; }

    protected abstract SimpleObjectType Type { get; }

    protected string Id => UniqueId.Id;

    public virtual void ReadProgress(GameProgress progress)
    {
        var simpleObjectsOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].SimpleObjectsDatas.SimpleObjectsOnScene;

        // we are build-in-scene object and it is first start of level
        if (!simpleObjectsOnScene.Dictionary.TryGetValue(Id, out var myState))
            return;

        // restore state
        transform.position = myState.Position.AsUnityVector();
    }

    public virtual void WriteToProgress(GameProgress progress)
    {
        var simpleObjectsOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].SimpleObjectsDatas.SimpleObjectsOnScene;

        // just to optimize
        simpleObjectsOnScene.Dictionary.TryGetValue(Id, out var data);
        if (data != null && !HasChangesBetweenSavedStateAndCurrentState(data))
            return;

        simpleObjectsOnScene.Dictionary[Id] = new SimpleObjectOnSceneData(
            transform.position.AsVectorData(),
            Type,
            SceneBuiltInItem
            );
    }

    protected bool HasChangesBetweenSavedStateAndCurrentState(SimpleObjectOnSceneData data)
    {
        return
            data.Position.AsUnityVector() != transform.position
            ;
    }
}
