using UnityEditor;
using UnityEngine;
using Code.Services;

namespace Code.Editor
{
    public class Tools
    {
        [MenuItem("Tools/Clear ALL Prefs")]
        public static void ClearAllPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        [MenuItem("Tools/Clear APP SETTINGS Prefs")]
        public static void ClearAppSettingsPrefs()
        {
            PlayerPrefs.DeleteKey(SaveLoadAppSettingsService.APP_SETTINGS_KEY);
            PlayerPrefs.Save();
        }

        [MenuItem("Tools/Clear PROGRESS Prefs")]
        public static void ClearProgressPrefs()
        {
            PlayerPrefs.DeleteKey(SaveLoadService.PROGRESS_KEY);
            PlayerPrefs.Save();
        }

        [MenuItem("Tools/Clear ANALYTIC Prefs")]
        public static void ClearAnalyticsPrefs()
        {
            PlayerPrefs.DeleteKey(SaveLoadAnalyticService.ANALYTIC);
            PlayerPrefs.Save();
        }
        
        [MenuItem("Tools/Set PROGRESS Prefs/Load 1.5.0 full completed game (before end of 1st location)")]
        public static void Load_1_5_0_Save_Before_FirstPrize()
        {
            string savePath = "Assets/Debug Saves/1.5.0 PJ VK save (before First Prize).json";
            string progressJSON = AssetDatabase.LoadAssetAtPath<TextAsset>(savePath).text; ;
            PlayerPrefs.SetString(SaveLoadService.PROGRESS_KEY, progressJSON);
            PlayerPrefs.Save();
        }
        
        [MenuItem("Tools/Set PROGRESS Prefs/Load 1.5.0 full completed game (Near First Prize)")]
        public static void Load_1_5_0_Save_Near_FirstPrize()
        {
            string savePath = "Assets/Debug Saves/1.5.0 PJ VK save (near First Prize).json";
            string progressJSON = AssetDatabase.LoadAssetAtPath<TextAsset>(savePath).text; ;
            PlayerPrefs.SetString(SaveLoadService.PROGRESS_KEY, progressJSON);
            PlayerPrefs.Save();
        }
    }
}