using System.Collections.Generic;
using Code.Services;
using UnityEngine;

namespace Code.Infrastructure
{
    public interface IGameFactory : IService
    {
        List<ISavedProgressReader> ProgressReaders { get; }
        List<ISavedProgressWriter> ProgressWriters { get; }
        GameObject CreateHero(GameObject at);
        Hud CreateHud();
        void Cleanup();
        void RegisterProgressWatchersExternal(GameObject gameObject);
        GameObject GetGameObject(string gameObjectId, Vector3 at, bool registerProgressWatchers = true);
        void Recycle(GameObject gameObject);
        ResourceSource CreateResourceSource(ResourceSourceType type, Vector3 at, bool registerProgressWatchers = true);
        ResourceStorage CreateResourceStorage(ResourceStorageType type, Vector3 at);
        SimpleObject CreateSimpleObject(SimpleObjectType type, Vector3 position);
        Workbench CreateWorkbench(Vector3 position);
        Chunk CreateChunk(Vector3 position);
        Workshop CreateWorkshop(WorkshopType type, Vector3 position);
        Converter CreateConverter(ConverterType type, Vector3 position);
        Dungeon CreateDungeon(string gameObjectId, Vector3 position);
    }
}