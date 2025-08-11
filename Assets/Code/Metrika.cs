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

        FirstStart          = 10,
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
    }

    private const string ENABLE_ANALYTICS = "EnableAnalytics";

    private static ISaveLoadAnalyticService _saveLoadAnalytic;
    private static AnalyticData _data;
    private static PlayTimer _playTimer;
    private static bool _gameLoaded = false;

    /// <summary>
    /// float - time in mins
    /// </summary>
    private static readonly Dictionary<float, Event> _playTimeEventMap = new()
    {
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
#if GAME_PUSH
        _isAnalyticsEnabled = GamePush.GP_Variables.GetBool(ENABLE_ANALYTICS);
#endif
        Logger.Log($"[Metrika] Initialized: AnalyticsEnabled={_isAnalyticsEnabled}");
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
        string eventName = $"{(int)eventType}_{eventType}";
#if !UNITY_EDITOR && UNITY_WEBGL && VK_GAMES
        ref bool field = ref GetEventData(eventType);
        if (_isAnalyticsEnabled && !field)
        {
            Analytics_Goal(eventName);
            field = true;
            Logger.Log($"[Metrika] EventReached: {eventName}. Send to Metrika");
            _saveLoadAnalytic.SaveAnalytic();

            return;
        }
#endif
    }

    /*
    public static void GoalReached(string @event, string value)
    {
        string eventName = $"{@event}_{value}";
#if !UNITY_EDITOR && UNITY_WEBGL && VK_GAMES
        if (_isAnalyticsEnabled)
            Analytics_Goal(eventName);
#endif
        Logger.Log($"[Metrika] complex GoalReached: {eventName}");
    }

    public static void GoalReached(string eventName, int value)
    {
        GoalReached(eventName, value.ToString());
    }*/

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
            case ConverterType.FurnaceConverter:
                break;
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
            case ToolType.None:
            default:
                break;
        }
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

            case Event.Game_Loaded:
                // to save runtime only to prevent multiple call runtime
                return ref _gameLoaded;
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
