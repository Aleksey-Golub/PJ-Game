using System;

namespace Assets.Code.Data
{
    [System.Serializable]
    public class UpgradeItemsProgress
    {
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