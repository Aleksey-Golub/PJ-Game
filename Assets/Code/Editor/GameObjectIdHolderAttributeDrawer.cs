using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(GameObjectIdHolderAttribute))]
public class GameObjectIdHolderAttributeDrawer : PropertyDrawer
{
    private int _choiceIndex = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            if (property.hasMultipleDifferentValues)
            {
                //EditorGUI.LabelField(position, label, new GUIContent("—"));
                EditorGUI.PropertyField(position, property);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            _choiceIndex = Array.IndexOf(GameObjectsIds.Ids, property.stringValue);
            _choiceIndex = _choiceIndex < 0 ? 0 : _choiceIndex;

            _choiceIndex = EditorGUILayout.Popup(label, _choiceIndex, GameObjectsIds.Ids);

            property.stringValue = GameObjectsIds.Ids[_choiceIndex];

            EditorGUI.LabelField(position, label, new GUIContent(property.stringValue));

            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.LabelField(position, "Use attribute with string field");
        }
    }
}
