using UnityEditor;
using UnityEngine;
using Code.Services;
using System.IO;

namespace Code.Editor
{
    public class Tools
    {
        [MenuItem("Tools/Clear ALL Prefs")]
        public static void ClearAllPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            Debug.Log("ALL Prefs Cleared");
        }

        [MenuItem("Tools/Clear APP SETTINGS Prefs")]
        public static void ClearAppSettingsPrefs()
        {
            PlayerPrefs.DeleteKey(SaveLoadAppSettingsService.APP_SETTINGS_KEY);
            PlayerPrefs.Save();

            Debug.Log("APP SETTINGS Prefs Cleared");
        }

        [MenuItem("Tools/Clear PROGRESS Prefs")]
        public static void ClearProgressPrefs()
        {
            PlayerPrefs.DeleteKey(SaveLoadService.PROGRESS_KEY);
            PlayerPrefs.Save();

            Debug.Log("PROGRESS Prefs Cleared");
        }

        [MenuItem("Tools/Clear ANALYTIC Prefs")]
        public static void ClearAnalyticsPrefs()
        {
            PlayerPrefs.DeleteKey(SaveLoadAnalyticService.ANALYTIC);
            PlayerPrefs.Save();

            Debug.Log("ANALYTIC Prefs Cleared");
        }
        
        [MenuItem("Tools/Get PROGRESS Prefs/Write to file")]
        public static void WriteProgressToEditorFile()
        {
            string folderPath = "/Debug Saves/Got Saves form prefs";
            string fileName = SaveLoadHelper.GetNowTimeToString().Replace('/', '-').Replace(':', '-');

            string json = PlayerPrefs.GetString(SaveLoadService.PROGRESS_KEY);
            string path = Application.dataPath + folderPath + $"/{fileName}.json";

            File.WriteAllText(path, json);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

            Debug.Log($"PROGRESS Prefs wrote into file='{path}'");
        }
        
        [MenuItem("Tools/Set PROGRESS Prefs/Load 1.5.0 full completed game (before end of 1st location)")]
        public static void Load_1_5_0_Save_Before_FirstPrize()
        {
            string savePath = "Assets/Debug Saves/1.5.0 PJ VK save (before First Prize).json";

            LoadSave(savePath);
        }
        
        [MenuItem("Tools/Set PROGRESS Prefs/Load 1.5.0 full completed game (Near First Prize)")]
        public static void Load_1_5_0_Save_Near_FirstPrize()
        {
            string savePath = "Assets/Debug Saves/1.5.0 PJ VK save (near First Prize).json";

            LoadSave(savePath);
        }
        
        [MenuItem("Tools/Set PROGRESS Prefs/Load 1.5.0 full completed game (on start 2nd location)")]
        public static void Load_1_5_0_Save_On_Start_2nd_Location()
        {
            string savePath = "Assets/Debug Saves/1.5.0 PJ VK save (on 2nd location).json";

            LoadSave(savePath);
        }

        private static void LoadSave(string savePath)
        {
            string progressJSON = AssetDatabase.LoadAssetAtPath<TextAsset>(savePath).text; ;
            PlayerPrefs.SetString(SaveLoadService.PROGRESS_KEY, progressJSON);
            PlayerPrefs.Save();

            Debug.Log($"save from '{savePath}' set into PROGRESS Prefs");
        }
    }
}