namespace Code.Data
{
    [System.Serializable]
    public class WorkshopOnSceneData : SingleUseConsumerBaseOnScene
    {
        public SpawnGameObjectData[] SpawnData;
        public WorkshopType Type;

        public WorkshopOnSceneData(
            Vector3Data position,
            bool sceneBuiltInItem,
            ResourceType needResourceType,
            int needResourceCount,
            int currentNeedResourceCount,
            SpawnGameObjectData[] spawnData,
            WorkshopType type
            ) : base(position, sceneBuiltInItem, needResourceType, needResourceCount, currentNeedResourceCount)
        {
            SpawnData = spawnData;
            Type = type;
        }
    }
}