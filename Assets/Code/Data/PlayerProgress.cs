namespace Assets.Code.Data
{
    [System.Serializable]
    public class PlayerProgress
    {
        public UpgradeItemsProgress UpgradeItemsProgress;

        public PlayerProgress()
        {
            UpgradeItemsProgress = new UpgradeItemsProgress();
        }
    }
}