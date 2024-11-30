namespace Code.Data
{
    [System.Serializable]
    public class PlayerProgress
    {
        public UpgradeItemsProgress UpgradeItemsProgress;
        public PositionOnLevel PositionOnLevel;
        public InventoryData InventoryData;
        public TimerData SpeedUpTimerData;
        public float SpeedUpSpeed;

        public PlayerProgress(string initialLevel)
        {
            UpgradeItemsProgress = new UpgradeItemsProgress();
            PositionOnLevel = new PositionOnLevel(initialLevel);
            InventoryData = new InventoryData();
            SpeedUpTimerData = new TimerData(false, 0, 0);
        }
    }
}