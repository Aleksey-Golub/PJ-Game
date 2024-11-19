namespace Code.Data
{
    [System.Serializable]
    public class DungeonOnSceneData
    {
        public Vector3Data Position;
        public bool SceneBuiltInItem;
        public string GameObjectId;
        public DungeonSpawnData[] SpawnData;
        public string[] ResourceSourcesIds;
        public DungeonEntranceOnScene EntranceState;
        public DungeonEntranceOnScene ExitState;

        public DungeonOnSceneData(
            Vector3Data position,
            bool sceneBuiltInItem,
            string gameObjectId,
            DungeonSpawnData[] spawnData,
            string[] resourceSourcesIds,
            DungeonEntranceOnScene entranceState,
            DungeonEntranceOnScene exitState
            )
        {
            Position = position;
            SceneBuiltInItem = sceneBuiltInItem;
            GameObjectId = gameObjectId;
            SpawnData = spawnData;
            ResourceSourcesIds = resourceSourcesIds;
            EntranceState = entranceState;
            ExitState = exitState;
        }
    }
}