namespace Code.Data
{
    [System.Serializable]
    public class PlayerProgress
    {
        public UpgradeItemsProgress UpgradeItemsProgress;
        public PositionOnLevel PositionOnLevel;

        public PlayerProgress(string initialLevel)
        {
            UpgradeItemsProgress = new UpgradeItemsProgress();
            PositionOnLevel = new PositionOnLevel(initialLevel);
        }
    }
}