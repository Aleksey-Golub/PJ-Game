namespace Assets.Code.Data
{
    [System.Serializable]
    public class UpgradeItemsProgress
    {
        /// <summary>
        /// Contains dictionary of [upgradableItemId, currentLevel] data. Default level is 0
        /// </summary>
        public UpgradeItemsDataDictionary UpgradeItemsData = new UpgradeItemsDataDictionary();

        internal void Upgrade(string itemID)
        {
            if (UpgradeItemsData.Dictionary.ContainsKey(itemID))
            {
                UpgradeItemsData.Dictionary[itemID] += 1;
            }
        }
    }
}