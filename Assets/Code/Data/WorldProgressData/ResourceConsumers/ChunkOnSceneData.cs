namespace Code.Data
{
    [System.Serializable]
    public class ChunkOnSceneData : SingleUseConsumerBaseOnScene
    {
        public SpawnGameObjectData[] SpawnData;
        public bool Opened;
        public bool OpenByOtherOnly;
        public bool DelayedOpenStart;
        public float DelayedOpenElapsedTime;

        public ChunkOnSceneData(
            Vector3Data position,
            bool sceneBuiltInItem,
            ResourceType needResourceType,
            int needResourceCount,
            int currentNeedResourceCount,
            SpawnGameObjectData[] spawnData,
            bool opened,
            bool openByOtherOnly,
            bool delayedOpenStart,
            float delayedOpenElapsedTime) : base(position, sceneBuiltInItem, needResourceType, needResourceCount, currentNeedResourceCount)
        {
            SpawnData = spawnData;
            Opened = opened;
            OpenByOtherOnly = openByOtherOnly;
            DelayedOpenStart = delayedOpenStart;
            DelayedOpenElapsedTime = delayedOpenElapsedTime;
        }
    }
}