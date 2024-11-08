using Code.Infrastructure;
using Code.Services;
using UnityEngine;

internal class ToolSceneSpawner : SceneSpawnerBase<ToolConfig>
{
    private IToolFactory _toolFactory;

    private void Start()
    {
        var factory = AllServices.Container.Single<IToolFactory>();
        var gameFactory = AllServices.Container.Single<IGameFactory>();
        
        Construct(factory, gameFactory);
    }

    private void Construct(IToolFactory toolFactory, IGameFactory gameFactory)
    {
        _toolFactory = toolFactory;

        gameFactory.RegisterProgressWatchers(gameObject);
    }

    protected override void SpawnInner()
    {
        foreach (var data in _spawnDatas)
        {
            var dropData = DropData.Get(data.Point.position, _dropSettings, data.Count, out int _);

            foreach (var dd in dropData)
            {
                Tool item = _toolFactory.Get(data.Point.position, Quaternion.identity);
                item.Init(data.Config as ToolConfig);

                item.MoveAfterDrop(dd);
            }
        }
    }
}
