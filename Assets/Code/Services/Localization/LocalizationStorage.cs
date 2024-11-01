using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "newLocalizationStorage", menuName = "Localization/Localization Storage")]
public class LocalizationStorage : ScriptableObject
{
    public List<LocRow> Rows;

    [System.Serializable]
    public struct LocRow
    {
        public string Key;
        public List<string> Values;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LocalizationStorage))]
    public class LocalizationStorageEditor : Editor
    {
        private const string SOURCE_PATH = "Assets/Localization Source/Pixel Journey - Localization.csv";
        //private const char ROW_SEPARATOR = '\n';
        private readonly string ROW_SEPARATOR = System.Environment.NewLine;
        private const char CELLS_SEPARATOR = ',';

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Update"))
                UpdateStorage();
        }

        private void UpdateStorage()
        {
            LocalizationStorage storage = (LocalizationStorage)target;
            storage.Rows.Clear();

            string sourceText = AssetDatabase.LoadAssetAtPath<TextAsset>(SOURCE_PATH).text;

            string[] sourceRows = sourceText.Split(ROW_SEPARATOR);

            foreach (string sourceRow in sourceRows)
            {
                List<string> list = new();
                string[] splited = sourceRow.Split(CELLS_SEPARATOR);

                for (int i = 1; i < splited.Length; i++)
                    list.Add(splited[i]);

                storage.Rows.Add(new LocRow() { Key = splited[0], Values = list });
            }

            EditorUtility.SetDirty(storage);
        }
    }
#endif
}