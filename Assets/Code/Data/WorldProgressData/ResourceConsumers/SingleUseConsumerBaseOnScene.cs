namespace Code.Data
{
    [System.Serializable]
    public abstract class SingleUseConsumerBaseOnScene
    {
        public Vector3Data Position;
        public bool SceneBuiltInItem;
        public ResourceType NeedResourceType;
        public int NeedResourceCount;
        public int CurrentNeedResourceCount;
        public bool IsAvailable;

        protected SingleUseConsumerBaseOnScene(
            Vector3Data position,
            bool sceneBuiltInItem,
            ResourceType needResourceType,
            int needResourceCount,
            int currentNeedResourceCount,
            bool isAvailable
            )
        {
            Position = position;
            SceneBuiltInItem = sceneBuiltInItem;
            NeedResourceType = needResourceType;
            NeedResourceCount = needResourceCount;
            CurrentNeedResourceCount = currentNeedResourceCount;
            IsAvailable = isAvailable;
        }
    }
}