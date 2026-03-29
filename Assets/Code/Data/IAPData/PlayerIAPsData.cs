using System;
using System.Collections.Generic;

namespace Code.Data
{
    [System.Serializable]
    public class PlayerIAPsData
    {
        public string SaveTime;
        public PurchasedIAPsData PurchasedIAPsData;

        public PlayerIAPsData()
        {
            PurchasedIAPsData = new PurchasedIAPsData();
        }
    }

    [System.Serializable]
    public class PurchasedIAPsData
    {
        public List<PurchasedIAPData> PurchasedIAPs = new();

        public void AddData(PurchasedIAPDTO purchasedIAPDTO)
        {
            TryAddEmptyData(purchasedIAPDTO.ProductIdOrTag, out var purchasedIAPData);

            purchasedIAPData.AddItem(new PurchasedIAPData.Item(purchasedIAPDTO.BoughtDate, purchasedIAPDTO.Source));
        }

        public PurchasedIAPData GetDataOrNull (string tag)
        {
            return PurchasedIAPs.Find(d => d.ProductIdOrTag == tag);
        }

        /// <summary>
        /// Return True if successfully add new data. Return false if data with Tag is present
        /// </summary>
        public bool TryAddEmptyData(string tag, out PurchasedIAPData data)
        {
            PurchasedIAPData purchasedIAPData = GetDataOrNull(tag);
            if (purchasedIAPData == null)
            {
                purchasedIAPData = new PurchasedIAPData(tag);
                PurchasedIAPs.Add(purchasedIAPData);
                data = purchasedIAPData;
                return true;
            }

            data = purchasedIAPData;
            return false;
        }
    }

    [System.Serializable]
    public class PurchasedIAPData
    {
        public string ProductIdOrTag;
        public List<Item> Datas = new();

        public PurchasedIAPData(string productIdOrTag)
        {
            ProductIdOrTag = productIdOrTag;
        }

        public void AddItem(Item item)
        {
            Datas.Add(item);
        }

        [System.Serializable]
        public class Item
        {
            public string BoughtDate;
            public PurchaseSource Source;

            public Item(string boughtDate, PurchaseSource source)
            {
                BoughtDate = boughtDate;
                Source = source;
            }

            public override string ToString()
            {
                return $"BoughtDate='{BoughtDate}', source='{Source}'";
            }
        }
    }

    [System.Serializable]
    public struct PurchasedIAPDTO
    {
        public string ProductIdOrTag;
        public string BoughtDate;
        public PurchaseSource Source;

        public PurchasedIAPDTO(string productIdOrTag, string boughtDate, PurchaseSource source)
        {
            ProductIdOrTag = productIdOrTag;
            BoughtDate = boughtDate;
            Source = source;
        }
    }

    [System.Serializable]
    public enum PurchaseSource
    {
        None = 0,

        /// <summary>
        /// Real player purchase that was handled correctly: save in data, granted resource etc.
        /// </summary>
        RealPurchase = 1,

        /// <summary>
        /// Unhandled purchase: player really purchase the good, but it was not handled correctly for some reasons.
        /// </summary>
        UnhandledPurchase = 2,

        /// <summary>
        /// Gift from server admin panel.
        /// </summary>
        Gift = 3,
    }
}
