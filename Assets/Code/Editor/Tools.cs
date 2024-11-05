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
            PlayerPrefs.DeleteKey(SaveLoadAppSettingsService.APP_SETTINGS_KEY);;
            PlayerPrefs.Save();
        }

        [MenuItem("Tools/Clear PROGRESS Prefs")]
        public static void ClearProgressPrefs()
        {
            PlayerPrefs.DeleteKey(SaveLoadService.PROGRESS_KEY);;
            PlayerPrefs.Save();
        }
    }
}