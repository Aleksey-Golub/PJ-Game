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
            ResourceType dropResourceType,
            ToolType dropToolType,
            int dropCount) : base(position, sceneBuiltInItem, needResourceType, needResourceCount, currentNeedResourceCount)
        {
            DropResourceType = dropResourceType;
            DropToolType = dropToolType;
            DropCount = dropCount;
        }
    }
}