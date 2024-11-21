namespace Code.Data
{
    [System.Serializable]
    public class AdsBoxOnSceneData : SimpleObjectOnSceneData
    {
        public ResourceType DropResourceType;
        public int DropResourceCount;

        public AdsBoxOnSceneData(
            Vector3Data position,
            SimpleObjectType type,
            bool sceneBuiltInItem,
            ResourceType dropResourceType,
            int dropResourceCount
            ) : base(position, type, sceneBuiltInItem)
        {
            DropResourceType = dropResourceType;
            DropResourceCount = dropResourceCount;
        }
    }
}