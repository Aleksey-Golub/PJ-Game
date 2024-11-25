using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "newLocalizationStorage", menuName = "Localization/Localization Storage")]
public class LocalizationStorage : ScriptableObject
{
    [Tooltip("Relative to project folder. Use forward slashes. For example: 'Assets/SomeFolder/text.csv'")]
    [SerializeField] private string _sourcePath = "Assets/Localization Source/Pixel Journey - Localization.csv";
    [SerializeField] private char _cellsSeparator = ',';

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
        private const string COMMENTS = "Comments";

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

            string sourceText = AssetDatabase.LoadAssetAtPath<TextAsset>(storage._sourcePath).text;

            sourceText = Prepare(sourceText);
            List<string[]> rows = GetRows(sourceText, storage._cellsSeparator);
            int commentsIndex = Array.IndexOf(rows[0], COMMENTS);

            foreach (string[] row in rows)
            {
                List<string> list = new();

                for (int i = 1; i < row.Length; i++)
                    // skip Comments
                    if (i != commentsIndex)
                        list.Add(row[i]);

                storage.Rows.Add(new LocRow() { Key = row[0], Values = list });
            }

            EditorUtility.SetDirty(storage);
        }

        private string Prepare(string sourceText)
        {
            sourceText = sourceText.Replace("\r\n", "\n");
            sourceText = sourceText.Replace('\r', '\n');

            return sourceText;
        }

        private List<string[]> GetRows(string sourceText, char separator)
        {
            List<string[]> rows = new();

            string[] firstRow = GetFirstRow(sourceText, separator, out int index);
            rows.Add(firstRow);

            index++;
            int textLength = sourceText.Length;
            int cellStart = index;
            bool quotations = false;
            char c;
            List<string> row = new List<string>(firstRow.Length);

            while (index < textLength)
            {
                c = sourceText[index];

                if (quotations)
                {
                    // do exit quotation
                    if (c == '\"')
                    {
                        // the last char
                        if (index + 1 >= textLength)
                        {
                            quotations = false;
                        }
                        // the single " is end of cell
                        else if (sourceText[index + 1] != '\"')
                        {
                            quotations = false;
                        }
                        // we are ", next char is " and third char is "
                        else if (index + 2 < textLength && sourceText[index + 2] == '\"')
                        {
                            quotations = false;
                            index += 2;
                        }
                        // we are " and next char is "
                        // this is not quotation end
                        else
                        {
                            index++;
                        }
                    }
                }
                else if (c == '\n' || c == separator)
                {
                    // add cell
                    AddCell(row, sourceText, index, ref cellStart);

                    // end of line
                    if (c == '\n')
                    {
                        rows.Add(row.ToArray());
                        row.Clear();
                    }
                }
                else if (c == '\"')
                {
                    quotations = true;
                }

                index++;
            }

            // the last cell in last row
            if (index > cellStart)
            {
                AddCell(row, sourceText, index, ref cellStart);
                rows.Add(row.ToArray());
            }

            // empty last cell
            if (row.Count < firstRow.Length)
            {
                row.Add(string.Empty);
                rows.Add(row.ToArray());
            }

            return rows;
        }

        private void AddCell(List<string> row, string sourceText, int index, ref int cellStart)
        {
            string cell = sourceText.Substring(cellStart, index - cellStart);
            cellStart = index + 1;

            cell = cell.Replace("\"\"", "\"");

            if (cell.Length > 1 && cell[0] == '\"' && cell[^1] == '\"')
                cell = cell.Substring(1, cell.Length - 2);

            row.Add(cell);
        }

        private string[] GetFirstRow(string sourceText, char separator, out int index)
        {
            index = sourceText.IndexOf('\n');
            return sourceText.Substring(0, index).Split(separator);
        }
    }
#endif
}