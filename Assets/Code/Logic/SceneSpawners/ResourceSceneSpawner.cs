using Code.Services;
using UnityEngine;

internal class ResourceSceneSpawner : SceneSpawnerBase<ResourceConfig>
{
    private IResourceFactory _resourceFactory;

    private void Start()
    {
        var factory = AllServices.Container.Single<IResourceFactory>();

        Construct(factory);
    }

    private void Construct(IResourceFactory resourceFactory)
    {
        _resourceFactory = resourceFactory;
    }

    protected override void SpawnInner()
    {
        foreach (var data in _spawnDatas)
        {
            var dropData = DropData.Get(data.Point.position, _dropSettings, data.Count, out int _);

            foreach (var dd in dropData)
            {
                Resource item = _resourceFactory.Get(data.Point.position, Quaternion.identity);
                item.Init(data.Config as ResourceConfig, dd.ResourceInPackCount);

                item.MoveAfterDrop(dd);
            }
        }
    }
}
