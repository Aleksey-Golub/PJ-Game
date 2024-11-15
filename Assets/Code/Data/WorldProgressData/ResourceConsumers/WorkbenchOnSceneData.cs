namespace Code.Data
{
    [System.Serializable]
    public class WorkbenchOnSceneData
    {
        public Vector3Data Position;
        public bool SceneBuiltInItem;
        public ResourceType NeedResourceType;
        public int NeedResourceCount;
        public int CurrentNeedResourceCount;
        public ResourceType DropResourceType;
        public ToolType DropToolType;
        public int DropCount;

        public WorkbenchOnSceneData(
            Vector3Data position,
            bool sceneBuiltInItem,
            ResourceType needResourceType,
            int needResourceCount,
            int currentNeeds,
            ResourceType dropResourceType,
            ToolType dropToolType,
            int dropCount)
        {
            Position = position;
            SceneBuiltInItem = sceneBuiltInItem;
            NeedResourceType = needResourceType;
            NeedResourceCount = needResourceCount;
            CurrentNeedResourceCount = currentNeeds;
            DropResourceType = dropResourceType;
            DropToolType = dropToolType;
            DropCount = dropCount;
        }
    }

    [System.Serializable]
    public class SingleUseConsumerBaseOnScene
    {

    }
}