namespace Code.Data
{
    [System.Serializable]
    public class ResourceStorageOnSceneData
    {
        public Vector3Data Position;
        public ResourceStorageType Type;
        public ResourceType DropResourceType;
        public int CurrentResourceCount;
        public float RestorationTimer;
        public bool SceneBuiltInItem;

        public ResourceStorageOnSceneData(
            Vector3Data position,
            ResourceStorageType type,
            ResourceType dropResourceType,
            int currentResourceCount,
            float restorationTimer,
            bool sceneBuiltInItem)
        {
            Position = position;
            Type = type;
            DropResourceType = dropResourceType;
            CurrentResourceCount = currentResourceCount;
            RestorationTimer = restorationTimer;
            SceneBuiltInItem = sceneBuiltInItem;
        }
    }
}