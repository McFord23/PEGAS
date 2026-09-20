using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RequireInterface))]
public class RequireInterfaceDrawer : PropertyDrawer
{
    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        var requireInterface = attribute as RequireInterface;
        var requireType = requireInterface.RequireType;
        if (IsValid(property, requireType))
        {
            label.tooltip = $"Require {requireInterface.RequireType} interface";
            CheckProperty(property, requireType);
        }
        EditorGUI.PropertyField(position, property, label);
    }

    private bool IsValid (SerializedProperty property, Type targetType)
    {
        return targetType.IsInterface && property.propertyType == SerializedPropertyType.ObjectReference;
    }

    private void CheckProperty (SerializedProperty property, Type targetType)
    {
        if (property.objectReferenceValue == null)
            return;
        if (property.objectReferenceValue as GameObject)
            CheckGameObject(property, targetType);
        else if (property.objectReferenceValue as ScriptableObject)
            CheckScriptableObject(property, targetType);
    }

    private void CheckGameObject (SerializedProperty property, Type targetType)
    {
        var field = property.objectReferenceValue as GameObject;
        if (field.GetComponent(targetType) == null)
        {
            property.objectReferenceValue = null;
            Debug.LogError($"GameObject must contain component implemented {targetType} interface");
        }
    }

    private void CheckScriptableObject (SerializedProperty property, Type targetType)
    {
        var field = property.objectReferenceValue as ScriptableObject;
        var fieldType = field.GetType();
        if (targetType.IsAssignableFrom(fieldType) == false)
        {
            property.objectReferenceValue = null;
            Debug.LogError($"ScriptableObject must implement {targetType} interface");
        }
    }
}