using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ForceInterfaceAttribute))]
public class ForceInterfaceAttributeDrawer : PropertyDrawer
{
    private const string GAME_OBJECT_PROPERTY_NAME = "PPtr<$GameObject>";
    private const string MONOBEHAVIOUR_PROPERTY_NAME = "PPtr<$MonoBehaviour>";
    private const string COMPONENT_PROPERTY_NAME = "PPtr<$Component>";
    private const string OBJECT_PROPERTY_NAME = "PPtr<$Object>";

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            if (property.type != GAME_OBJECT_PROPERTY_NAME &&
                property.type != MONOBEHAVIOUR_PROPERTY_NAME &&
                property.type != COMPONENT_PROPERTY_NAME &&
                property.type != OBJECT_PROPERTY_NAME)
            {
                EditorGUI.LabelField(position, $"Use {nameof(ForceInterfaceAttribute)} with Object, Monobehaviour, GameObject or Component field types only.");
                return;
            }

            ForceInterfaceAttribute forceAttribute = (ForceInterfaceAttribute)attribute;
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            Object obj = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(Object), !EditorUtility.IsPersistent(property.serializedObject.targetObject));

            if (EditorGUI.EndChangeCheck())
            {
                if (obj == null)
                {
                    property.objectReferenceValue = null;
                }
                else if (forceAttribute.InterfaceType.IsAssignableFrom(obj.GetType()))
                {
                    if (property.type is GAME_OBJECT_PROPERTY_NAME)
                    {
                        property.objectReferenceValue = (obj as Component).gameObject;
                    }
                    else if (property.type is MONOBEHAVIOUR_PROPERTY_NAME or COMPONENT_PROPERTY_NAME)
                    {
                        property.objectReferenceValue = obj;
                    }
                    else if (property.type is OBJECT_PROPERTY_NAME)
                    {
                        Debug.LogWarning($"5 {property.name} {property.type}");
                        property.objectReferenceValue = obj;
                    }
                    else
                    {
                        Debug.LogWarning($"Unhandled behaviour for property.type='{property.type}' and component type = {obj.GetType()}. Nothing happened");
                    }
                }
                else if (obj is GameObject)
                {
                    MonoBehaviour mono = ((GameObject)obj).GetComponent(forceAttribute.InterfaceType) as MonoBehaviour;
                    if (mono != null)
                    {
                        if (property.type is GAME_OBJECT_PROPERTY_NAME)
                        {
                            property.objectReferenceValue = obj;
                        }
                        else
                        {
                            property.objectReferenceValue = mono;
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Use only with interface [{forceAttribute.InterfaceType.Name}]");
                    }
                }
                else
                {
                    Debug.LogWarning($"Use only with interface [{forceAttribute.InterfaceType.Name}]");
                }
            }

            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.LabelField(position, $"Use {nameof(ForceInterfaceAttribute)} on Object");
        }
    }
}
