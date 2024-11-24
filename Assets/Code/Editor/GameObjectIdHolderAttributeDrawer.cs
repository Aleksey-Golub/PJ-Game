using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(GameObjectIdHolderAttribute))]
public class GameObjectIdHolderAttributeDrawer : PropertyDrawer
{
    private int _choiceIndex = 0;
    private string[] _ids;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            if (_ids == null)
            {
                FillArray();
            }

            if (property.hasMultipleDifferentValues)
            {
                //EditorGUI.LabelField(position, label, new GUIContent("—"));
                EditorGUI.PropertyField(position, property);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            _choiceIndex = Array.IndexOf(_ids, property.stringValue);
            _choiceIndex = _choiceIndex < 0 ? 0 : _choiceIndex;

            _choiceIndex = EditorGUILayout.Popup(label, _choiceIndex, _ids);

            property.stringValue = _ids[_choiceIndex];

            EditorGUI.LabelField(position, label, new GUIContent(property.stringValue));

            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.LabelField(position, "Use attribute with string field");
        }
    }

    private void FillArray()
    {
        List<string> ids = new(10);
        GameObjectType validTypes = ((GameObjectIdHolderAttribute)attribute).GameObjectType;

        foreach (GameObjectType t in Enum.GetValues(typeof(GameObjectType)))
        {
            if (t is GameObjectType.All or GameObjectType.None)
                continue;

            if (!validTypes.HasFlag(t))
                continue;

            switch (t)
            {
                case GameObjectType.SimpleObject:
                    ids.AddRange(G_GameObjectsIds_SimpleObject.Ids);
                    break;
                case GameObjectType.ResourceSource:
                    ids.AddRange(G_GameObjectsIds_ResourceSource.Ids);
                    break;
                case GameObjectType.ResourceStorage:
                    ids.AddRange(G_GameObjectsIds_ResourceStorage.Ids);
                    break;
                case GameObjectType.ResourceConsumer:
                    ids.AddRange(G_GameObjectsIds_ResourceConsumer.Ids);
                    break;
                case GameObjectType.Dungeon:
                    ids.AddRange(G_GameObjectsIds_Dungeon.Ids);
                    break;
                case GameObjectType.Special:
                    ids.AddRange(G_GameObjectsIds_Special.Ids);
                    break;
                case GameObjectType.All:
                case GameObjectType.None:
                default:
                    Debug.LogError($"Not inplemented for {t}. Return 'special' ids");
                    ids.Add("not implemented");
                    break;
            }
        }

        _ids = ids.ToArray();
    }
}
