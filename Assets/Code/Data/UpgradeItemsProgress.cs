using System;

namespace Assets.Code.Data
{
    [System.Serializable]
    public class UpgradeItemsProgress
    {
        /// <summary>
        /// Contains dictionary of [upgradableItemId, currentLevel] data. Default level is 0
        /// </summary>
        public UpgradeItemsDataDictionary UpgradeItemsData = new UpgradeItemsDataDictionary();
        public event Action<string, int> Changed;

        internal void Upgrade(string itemID)
        {
            if (UpgradeItemsData.Dictionary.ContainsKey(itemID))
            {
                UpgradeItemsData.Dictionary[itemID] += 1;
            }
            else
            {
                UpgradeItemsData.Dictionary[itemID] = 1;
            }

            Changed?.Invoke(itemID, UpgradeItemsData.Dictionary[itemID]);
        }

        internal void Set(string itemID, int value)
        {
            UpgradeItemsData.Dictionary[itemID] = value;

            Changed?.Invoke(itemID, UpgradeItemsData.Dictionary[itemID]);
        }

        internal bool TryGet(string itemID, out int value)
        {
            return UpgradeItemsData.Dictionary.TryGetValue(itemID, out value);
        }
    }
}