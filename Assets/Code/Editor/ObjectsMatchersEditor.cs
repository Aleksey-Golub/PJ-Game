using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectsMatchers))]
public class ObjectsMatchersEditor : Editor
{
    private ObjectsMatchers _target;

    private void OnEnable()
    {
        _target = (ObjectsMatchers)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Ids"))
        {
            List<string> ids = new();
            foreach (GameObjectMatcher matcher in _target.Configs)
            {
                string id = matcher.GameObjectId;
                if (!ids.Contains(id))
                {
                    ids.Add(id);
                }
                else
                {
                    Debug.LogError($"Id= {id} for '{matcher.Template.name}' already exists for gameObject= '{_target.Configs.Find(m => m.GameObjectId == id).Template.name}'. Ids have to be unique.\nGenerating stopped.");
                    return;
                }
            }

            //ids.Sort();
            ids.Insert(0, "none");
            Generate("Assets/Code/Editor/Generated/GameObjectsIds.cs", "GameObjectsIds", ids);
        }
    }

    private void Generate(string path, string @class, List<string> ids)
    {
        var builder = new StringBuilder();

        builder.AppendLine($"public static class {@class} \n{{");
        builder.AppendLine($"    public static readonly string[] Ids = new string[]\n    {{");

        for (int i = 0; i < ids.Count; i++)
        {
            if (i != ids.Count - 1)
                builder.AppendLine($"        \"{ids[i]}\",");
            else
                builder.AppendLine($"        \"{ids[i]}\"");
        }

        builder.AppendLine($"    }};\n}}");

        WriteText(path, builder.ToString());

        Debug.Log("Ids generating success!");
    }

    private static void WriteText(string path, string text)
    {
        try
        {
            if (!File.Exists(path) || File.ReadAllText(path) != text)
            {
                File.WriteAllText(path, text);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.Default);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Exception while generate: {e.Message}");
        }
    }
}
