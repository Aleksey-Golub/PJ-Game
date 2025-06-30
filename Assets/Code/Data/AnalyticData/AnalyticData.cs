using System;

namespace Code.Data
{
    [Serializable]
    public class AnalyticData
    {
        public AnalyticEventsData AnalyticEventsData;
        public string SaveTime;

        public AnalyticData()
        {
            AnalyticEventsData = new AnalyticEventsData();
        }
    }

    [Serializable]
    public class AnalyticEventsData
    {
        public bool e_dummy;

        public bool e_FirstStart;
        public bool e_CollectSickle;
        public bool e_CollectAxe;
        public bool e_FirstUpgradeSickle;
        public bool e_CollectPickAxe;
        public bool e_BuyCow;
        public bool e_CollectSword;
        public bool e_FirstCarrot;
        public bool e_BuyPig;
        public bool e_CollectBucket;
        public bool e_FirstFruitBush;
        public bool e_OpenFirstPrizeChunk;

        public float playTimeSeconds;
        public bool e_Play_5_Min;
        public bool e_Play_10_Min;
        public bool e_Play_15_Min;
        public bool e_Play_20_Min;
        public bool e_Play_25_Min;
        public bool e_Play_30_Min;
        public bool e_Play_40_Min;
        public bool e_Play_50_Min;
        public bool e_Play_60_Min;
        public bool e_Play_120_Min;
        public bool e_Play_180_Min;
    }
}