using Code.Data;
using Code.Services;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
#if !UNITY_EDITOR && UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

/// <summary>
/// Yandex Metrika Analytics.
/// Use GamePush variables to on/off analytics remote.
/// </summary>
public class Metrika : MonoBehaviour
{
    public enum Event
    {
        None                = 0,
        Game_Loaded         = 1,

        // 1st location
        FirstStart = 10,
        CollectSickle       = 20,
        CollectAxe          = 30,
        FirstUpgradeSickle  = 40,
        CollectPickAxe      = 50,
        BuyCow              = 60,
        CollectSword        = 70,
        FirstCarrot         = 80,
        BuyPig              = 90,
        CollectBucket       = 100,
        FirstFruitBush      = 110,
        OpenFirstPrizeChunk = 120,

        // 2nd location
        OpenFirstChunk_2ndLocation          = 130,
        OpenFirstDiamondChunk_2ndLocation   = 140, // 22,10
        OpenFirstCoalChunk_2ndLocation      = 150, // for 3 coal, -10,14
        OpenSecondIronOreChunk_2ndLocation  = 160, // for 15 iron ore, -6,30
        CollectScimitar                     = 170,
        OpenPhoenixTreeChunk_2ndLocation    = 180, // for 10 iron, 6,30
        OpenChunkWithTornado_2ndLocation    = 190,
        OpenFirstPoppyChunk_2ndLocation     = 200, // for 2 poppy, 18,22
        OpenFinalBridgeChunk_2ndLocation    = 210, // for 30 poppy, 6,34
        BuySecondPrize_2ndLocation          = 220,

        // ads events
        Ads_Interstitial_Successed  = 900,
        Ads_Rewarded_Successed      = 901,
        Ads_Premium_Bought          = 902,
        Ads_Support_Bought          = 903,

        // dont forget to add it to <time, Event> dictionary
        Play_5_Min   = 1005,
        Play_10_Min  = 1010,
        Play_15_Min  = 1015,
        Play_20_Min  = 1020,
        Play_25_Min  = 1025,
        Play_30_Min  = 1030,
        Play_40_Min  = 1040,
        Play_50_Min  = 1050,
        Play_60_Min  = 1060,
        Play_90_Min  = 1090,
        Play_120_Min = 1120,
        Play_150_Min = 1150,
        Play_180_Min = 1180,
        Play_210_Min = 1210,
        Play_240_Min = 1240,
        Play_270_Min = 1270,
        Play_300_Min = 1300,
    }

    private static string _enableAnalyticsKey = "EnableAnalytics_DEFAULT";
    private static bool _isAnalyticsAvailable = false;

    private static ISaveLoadAnalyticService _saveLoadAnalytic;
    private static AnalyticData _data;
    private static PlayTimer _playTimer;
    private static bool _gameLoaded = false;

    /// <summary>
    /// float - time in mins
    /// </summary>
    private static readonly Dictionary<float, Event> _playTimeEventMap = new()
    {
        { 300f, Event.Play_300_Min },
        { 270f, Event.Play_270_Min },
        { 240f, Event.Play_240_Min },
        { 210f, Event.Play_210_Min },
        { 180f, Event.Play_180_Min },
        { 150f, Event.Play_150_Min },
        { 120f, Event.Play_120_Min },
        {  90f, Event.Play_90_Min },
        {  60f, Event.Play_60_Min },
        {  50f, Event.Play_50_Min },
        {  40f, Event.Play_40_Min },
        {  30f, Event.Play_30_Min },
        {  25f, Event.Play_25_Min },
        {  20f, Event.Play_20_Min },
        {  15f, Event.Play_15_Min },
        {  10f, Event.Play_10_Min },
        {   5f, Event.Play_5_Min },
    };

    private static bool _isAnalyticsEnabled;

#if !UNITY_EDITOR && UNITY_WEBGL

    [DllImport("__Internal")]
    private static extern void Analytics_Goal(string eventName);

#endif

    public static void Initialize(ISaveLoadAnalyticService saveLoadAnalytic, AnalyticData data)
    {
        _saveLoadAnalytic = saveLoadAnalytic;
        _data = data;

        SetPlatformSettings();

        if (_isAnalyticsAvailable)
        {
#if GAME_PUSH
            _isAnalyticsEnabled = GamePush.GP_Variables.GetBool(_enableAnalyticsKey);
#endif
        }

        Logger.Log($"[Metrika] Initialized: AnalyticsEnabled={_isAnalyticsEnabled}");

        static void SetPlatformSettings()
        {
#if GAME_PUSH
            if (GamePush.GP_Platform.Type() is GamePush.Platform.VK)
            {
                _enableAnalyticsKey = "EnableAnalytics_VK";
                _isAnalyticsAvailable = true;
            }
#endif
        }
    }

    public static void StartPlayTimer()
    {
        _playTimer = new PlayTimer(_data);
        _playTimer.StartTimer();
    }

    private void Start()
    {
        PlatformLayer.WebGlWindowClosedOrRefreshed += OnWebGlWindowClosedOrRefreshed;
    }

    private void Update()
    {
        _playTimer?.OnUpdate(Time.deltaTime);
    }

    private void OnDestroy()
    {
        PlatformLayer.WebGlWindowClosedOrRefreshed -= OnWebGlWindowClosedOrRefreshed;
    }
    private void OnWebGlWindowClosedOrRefreshed() => _saveLoadAnalytic.SaveAnalytic();

    public static void EventReached(Event eventType)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        string eventName = $"{(int)eventType}_{eventType}";
        if (GamePush.GP_Platform.Type() is GamePush.Platform.VK)
        {
            ref bool field = ref GetEventData(eventType);
            if (_isAnalyticsEnabled && !field)
            {
                Analytics_Goal(eventName);
                field = true;
                Logger.Log($"[Metrika] EventReached: {eventName}. Send to Metrika");
                _saveLoadAnalytic.SaveAnalytic();

                return;
            }
        }
#endif
    }

    internal static void ItemUpgradeded(string itemId, int currentLevel)
    {
        // first time upgrade
        if (currentLevel == 1)
        {
            if (Enum.TryParse(itemId, out ToolType toolType))
            {
                switch (toolType)
                {
                    case ToolType.SICKLE:
                        Metrika.EventReached(Metrika.Event.FirstUpgradeSickle);
                        break;
                    case ToolType.AXE:
                        break;
                    case ToolType.PICKAXE:
                        break;
                    case ToolType.SWORD:
                        break;
                    case ToolType.BUCKET:
                        break;
                    case ToolType.SCIMITAR:
                        break;
                    case ToolType.None:
                    default:
                        break;
                }
            }
            else if (Enum.TryParse(itemId, out ResourceStorageType storageType))
            {

            }
            else if (Enum.TryParse(itemId, out ConverterType converterType))
            {

            }
        }
    }

    internal static void WorkshopBuilt(WorkshopType type)
    {
        switch (type)
        {
            case WorkshopType.WorkshopBase:
                break;
            case WorkshopType.DryFruitBush:
                Metrika.EventReached(Metrika.Event.FirstFruitBush);
                break;
            case WorkshopType.LittleBush:
            case WorkshopType.LittleDesertTree:
            case WorkshopType.LittlePoppyBush:
            case WorkshopType.Bridge_Partial_CrackedSupportSouth:
            case WorkshopType.Special_Second_Prize_Spawner:
            case WorkshopType.PointForPlanting:
            case WorkshopType.LittlePhoenixTree:
            case WorkshopType.Bridge_Partial_CrackedSupportNorth:
            case WorkshopType.Bridge_Partial_RestoredSupports:
            case WorkshopType.Bridge_Partial_FirstRopes:
            case WorkshopType.Bridge_Partial_WoodWithoutNails:
            case WorkshopType.Bridge_Partial_WoodWithNails:
            case WorkshopType.None:
            default:
                break;
        }
    }

    internal static void ConverterBought(ConverterType type)
    {
        switch (type)
        {
            case ConverterType.CowConverter:
                Metrika.EventReached(Metrika.Event.BuyCow);
                break;
            case ConverterType.PigConverter:
                Metrika.EventReached(Metrika.Event.BuyPig);
                break;
            case ConverterType.FurnaceCoalConverter:
            case ConverterType.FurnaceIronConverter:
            case ConverterType.None:
            default:
                break;
        }
    }

    internal static void ResourceAddedInInventory(ResourceType type, int value)
    {
        switch (type)
        {
            case ResourceType.COIN:
                break;
            case ResourceType.GEM:
                break;
            case ResourceType.GRASS:
                break;
            case ResourceType.WOOD:
                break;
            case ResourceType.STONE:
                break;
            case ResourceType.MILK:
                break;
            case ResourceType.SLIME_EGG:
                break;
            case ResourceType.CARROT:
                Metrika.EventReached(Metrika.Event.FirstCarrot);
                break;
            case ResourceType.DUNG:
                break;
            case ResourceType.WATER:
                break;
            case ResourceType.FRUIT:
                break;
            case ResourceType.IRON_ORE:
                break;
            case ResourceType.IRON:
                break;
            case ResourceType.PHOENIX:
                break;
            case ResourceType.SLIME_EGG_DESERT:
                break;
            case ResourceType.SANDSTONE:
                break;
            case ResourceType.COAL:
                break;
            case ResourceType.POPPY:
                break;
            case ResourceType.None:
            default:
                break;
        }
    }

    internal static void ToolCollected(Tool tool)
    {
        switch (tool.Type)
        {
            case ToolType.SICKLE:
                Metrika.EventReached(Metrika.Event.CollectSickle);
                break;
            case ToolType.AXE:
                Metrika.EventReached(Metrika.Event.CollectAxe);
                break;
            case ToolType.PICKAXE:
                Metrika.EventReached(Metrika.Event.CollectPickAxe);
                break;
            case ToolType.SWORD:
                Metrika.EventReached(Metrika.Event.CollectSword);
                break;
            case ToolType.BUCKET:
                Metrika.EventReached(Metrika.Event.CollectBucket);
                break;
            case ToolType.SCIMITAR:
                Metrika.EventReached(Metrika.Event.CollectScimitar);
                break;
            case ToolType.None:
            default:
                break;
        }
    }

    internal static void Event_AdsInterstitialSuccessed()
    {
        _data.AnalyticEventsData.e_Ads_Interstitial_Successed++;

#if GAME_PUSH
        if (GamePush.GP_Platform.Type() is GamePush.Platform.VK)
        {
            string eventName = Metrika.Event.Ads_Interstitial_Successed.ToString();
            for (int i = _data.AnalyticEventsData.e_Ads_Interstitial_Successed_Tracked + 1; i <= _data.AnalyticEventsData.e_Ads_Interstitial_Successed; i++)
            {
                GamePush.GP_Analytics.Goal(eventName, i);
            }
            _data.AnalyticEventsData.e_Ads_Interstitial_Successed_Tracked = _data.AnalyticEventsData.e_Ads_Interstitial_Successed;
        }
#endif

        Metrika.EventReached(Metrika.Event.Ads_Interstitial_Successed);
    }

    internal static void Event_Ads_Rewarded_Successed()
    {
        _data.AnalyticEventsData.e_Ads_Rewarded_Successed++;

#if GAME_PUSH
        if (GamePush.GP_Platform.Type() is GamePush.Platform.VK)
        {
            string eventName = Metrika.Event.Ads_Rewarded_Successed.ToString();
            for (int i = _data.AnalyticEventsData.e_Ads_Rewarded_Successed_Tracked + 1; i <= _data.AnalyticEventsData.e_Ads_Rewarded_Successed; i++)
            {
                GamePush.GP_Analytics.Goal(eventName, i);
            }
            _data.AnalyticEventsData.e_Ads_Rewarded_Successed_Tracked = _data.AnalyticEventsData.e_Ads_Rewarded_Successed;
        }
#endif

        Metrika.EventReached(Metrika.Event.Ads_Rewarded_Successed);
    }

    private static void OnPlayTimeChanged(float playTimeSeconds)
    {
        foreach (KeyValuePair<float, Event> pair in _playTimeEventMap)
            if (playTimeSeconds > pair.Key * 60)
                EventReached(pair.Value);
    }

    [UsedImplicitly]
    private static ref bool GetEventData(Event @event)
    {
        // use e_dummy field to send events multiple times
        _data.AnalyticEventsData.e_dummy = false;

        switch (@event)
        {
            // 1st location
            case Event.FirstStart:
                return ref _data.AnalyticEventsData.e_FirstStart;
            case Event.CollectSickle:
                return ref _data.AnalyticEventsData.e_CollectSickle;
            case Event.CollectAxe:
                return ref _data.AnalyticEventsData.e_CollectAxe;
            case Event.FirstUpgradeSickle:
                return ref _data.AnalyticEventsData.e_FirstUpgradeSickle;
            case Event.CollectPickAxe:
                return ref _data.AnalyticEventsData.e_CollectPickAxe;
            case Event.BuyCow:
                return ref _data.AnalyticEventsData.e_BuyCow;
            case Event.CollectSword:
                return ref _data.AnalyticEventsData.e_CollectSword;
            case Event.FirstCarrot:
                return ref _data.AnalyticEventsData.e_FirstCarrot;
            case Event.BuyPig:
                return ref _data.AnalyticEventsData.e_BuyPig;
            case Event.CollectBucket:
                return ref _data.AnalyticEventsData.e_CollectBucket;
            case Event.FirstFruitBush:
                return ref _data.AnalyticEventsData.e_FirstFruitBush;
            case Event.OpenFirstPrizeChunk:
                return ref _data.AnalyticEventsData.e_OpenFirstPrizeChunk;

            // 2nd location
            case Event.OpenFirstChunk_2ndLocation:
                return ref _data.AnalyticEventsData.e_OpenFirstChunk_2ndLocation;
            case Event.OpenFirstDiamondChunk_2ndLocation:
                return ref _data.AnalyticEventsData.e_OpenFirstDiamondChunk_2ndLocation;
            case Event.OpenFirstCoalChunk_2ndLocation:
                return ref _data.AnalyticEventsData.e_OpenFirstCoalChunk_2ndLocation;
            case Event.OpenSecondIronOreChunk_2ndLocation:
                return ref _data.AnalyticEventsData.e_OpenSecondIronOreChunk_2ndLocation;
            case Event.CollectScimitar:
                return ref _data.AnalyticEventsData.e_CollectScimitar;
            case Event.OpenPhoenixTreeChunk_2ndLocation:
                return ref _data.AnalyticEventsData.e_OpenPhoenixTreeChunk_2ndLocation;
            case Event.OpenChunkWithTornado_2ndLocation:
                return ref _data.AnalyticEventsData.e_OpenChunkWithTornado_2ndLocation;
            case Event.OpenFirstPoppyChunk_2ndLocation:
                return ref _data.AnalyticEventsData.e_OpenFirstPoppyChunk_2ndLocation;
            case Event.OpenFinalBridgeChunk_2ndLocation:
                return ref _data.AnalyticEventsData.e_OpenFinalBridgeChunk_2ndLocation;
            case Event.BuySecondPrize_2ndLocation:
                return ref _data.AnalyticEventsData.e_BuySecondPrize_2ndLocation;

            // playtime
            case Event.Play_5_Min:
                return ref _data.AnalyticEventsData.e_Play_5_Min;
            case Event.Play_10_Min:
                return ref _data.AnalyticEventsData.e_Play_10_Min;
            case Event.Play_15_Min:
                return ref _data.AnalyticEventsData.e_Play_15_Min;
            case Event.Play_20_Min:
                return ref _data.AnalyticEventsData.e_Play_20_Min;
            case Event.Play_25_Min:
                return ref _data.AnalyticEventsData.e_Play_25_Min;
            case Event.Play_30_Min:
                return ref _data.AnalyticEventsData.e_Play_30_Min;
            case Event.Play_40_Min:
                return ref _data.AnalyticEventsData.e_Play_40_Min;
            case Event.Play_50_Min:
                return ref _data.AnalyticEventsData.e_Play_50_Min;
            case Event.Play_60_Min:
                return ref _data.AnalyticEventsData.e_Play_60_Min;
            case Event.Play_90_Min:
                return ref _data.AnalyticEventsData.e_Play_90_Min;
            case Event.Play_120_Min:
                return ref _data.AnalyticEventsData.e_Play_120_Min;
            case Event.Play_150_Min:
                return ref _data.AnalyticEventsData.e_Play_150_Min;
            case Event.Play_180_Min:
                return ref _data.AnalyticEventsData.e_Play_180_Min;
            case Event.Play_210_Min:
                return ref _data.AnalyticEventsData.e_Play_210_Min;
            case Event.Play_240_Min:
                return ref _data.AnalyticEventsData.e_Play_240_Min;
            case Event.Play_270_Min:
                return ref _data.AnalyticEventsData.e_Play_270_Min;
            case Event.Play_300_Min:
                return ref _data.AnalyticEventsData.e_Play_300_Min;

            case Event.Game_Loaded:
                // to save runtime only to prevent multiple call runtime
                return ref _gameLoaded;

            // ads events
            case Event.Ads_Interstitial_Successed:
                return ref _data.AnalyticEventsData.e_dummy;
            case Event.Ads_Rewarded_Successed:
                return ref _data.AnalyticEventsData.e_dummy;
            case Event.Ads_Premium_Bought:
                return ref _data.AnalyticEventsData.e_Ads_Premium_Bought;
            case Event.Ads_Support_Bought:
                return ref _data.AnalyticEventsData.e_Ads_Support_Bought;
            case Event.None:
            default:
                Logger.LogWarning($"[Metrika.NeedToSendEvent] unhandled event='{@event}'");
                return ref _data.AnalyticEventsData.e_dummy;
        }
    }

    private class PlayTimer
    {
        private readonly float _timerIntervalSeconds = 60f;

        private readonly Timer _playTimer;
        private readonly AnalyticData _analyticData;

        public PlayTimer(AnalyticData analyticData)
        {
            _playTimer = new Timer();
            _analyticData = analyticData;

            _playTimer.Elapsed += SetPlayTime;
        }

        public void StartTimer()
        {
            _playTimer.Start(_timerIntervalSeconds);
        }

        public void OnUpdate(float deltaTime)
        {
            _playTimer.OnUpdate(deltaTime);
        }

        private void SetPlayTime(Timer timer)
        {
            _analyticData.AnalyticEventsData.playTimeSeconds += _timerIntervalSeconds;
            Metrika.OnPlayTimeChanged(_analyticData.AnalyticEventsData.playTimeSeconds);

            StartTimer();
        }
    }
}
