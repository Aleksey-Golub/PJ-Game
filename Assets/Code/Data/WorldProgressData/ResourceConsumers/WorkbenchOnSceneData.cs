namespace Code.Data
{
    [System.Serializable]
    public class WorkbenchOnSceneData : SingleUseConsumerBaseOnScene
    {
        public ResourceType DropResourceType;
        public ToolType DropToolType;
        public int DropCount;

        public WorkbenchOnSceneData(
            Vector3Data position,
            bool sceneBuiltInItem,
            ResourceType needResourceType,
            int needResourceCount,
            int currentNeedResourceCount,
            bool isAvailable,
            ResourceType dropResourceType,
            ToolType dropToolType,
            int dropCount
            ) : base(position, sceneBuiltInItem, needResourceType, needResourceCount, currentNeedResourceCount, isAvailable)
        {
            DropResourceType = dropResourceType;
            DropToolType = dropToolType;
            DropCount = dropCount;
        }
    }
}