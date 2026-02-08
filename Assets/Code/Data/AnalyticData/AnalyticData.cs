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

        // 1st location
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

        // 2nd location
        public bool e_OpenFirstChunk_2ndLocation;
        public bool e_OpenFirstDiamondChunk_2ndLocation;
        public bool e_OpenFirstCoalChunk_2ndLocation;
        public bool e_OpenSecondIronOreChunk_2ndLocation;
        public bool e_CollectScimitar;
        public bool e_OpenPhoenixTreeChunk_2ndLocation;
        public bool e_OpenChunkWithTornado_2ndLocation;
        public bool e_OpenFirstPoppyChunk_2ndLocation;
        public bool e_OpenFinalBridgeChunk_2ndLocation;
        public bool e_BuySecondPrize_2ndLocation;

        // ads
        /// <summary> Amount of successfully watched Interstitial ads </summary>
        public int e_Ads_Interstitial_Successed;
        /// <summary> Amount of tracked with Analytic Interstitial ads </summary>
        public int e_Ads_Interstitial_Successed_Tracked;
        /// <summary> Amount of successfully watched Rewarded ads </summary>
        public int e_Ads_Rewarded_Successed;
        /// <summary> Amount of tracked with Analytic Rewarded ads </summary>
        public int e_Ads_Rewarded_Successed_Tracked;
        public bool e_Ads_Premium_Bought;
        public bool e_Ads_Support_Bought;

        // playtime
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
        public bool e_Play_90_Min;
        public bool e_Play_120_Min;
        public bool e_Play_150_Min;
        public bool e_Play_180_Min;
        public bool e_Play_210_Min;
        public bool e_Play_240_Min;
        public bool e_Play_270_Min;
        public bool e_Play_300_Min;
    }
}