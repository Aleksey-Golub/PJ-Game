namespace Code.Data
{
    [System.Serializable]
    public class PlayerProgress
    {
        public UpgradeItemsProgress UpgradeItemsProgress;
        public PositionOnLevel PositionOnLevel;
        public InventoryData InventoryData;

        public PlayerProgress(string initialLevel)
        {
            UpgradeItemsProgress = new UpgradeItemsProgress();
            PositionOnLevel = new PositionOnLevel(initialLevel);
            InventoryData = new InventoryData();
        }
    }
}