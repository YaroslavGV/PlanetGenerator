using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof (RequireInterfaceAttribute))]
sealed class RequireInterfaceAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            if (property.objectReferenceValue != null)
            {
                RequireInterfaceAttribute riAttribute = attribute as RequireInterfaceAttribute;
                HandleProperty(property, riAttribute.type);
            }
            EditorGUI.PropertyField(position, property, label);
        }
        else
        {
            EditorGUI.LabelField(position, label, new GUIContent("Use RequireInterface with reference object."));
        }
    }

    private void HandleProperty (SerializedProperty property, Type interfaceType)
    {
        if (property.objectReferenceValue is GameObject)
            HandleGameObject(property, interfaceType);
        else if (property.objectReferenceValue is Component)
            HandleComponent(property, interfaceType);
        else
            HandleGeneric(property, interfaceType);
    }

    private void HandleGameObject (SerializedProperty property, Type interfaceType)
    {
        GameObject gameObject = property.objectReferenceValue as GameObject;
        Component component = gameObject.GetComponent(interfaceType);
        if (component == null)
        {
            Debug.LogError(component.gameObject.name+" does not contain component implemented "+interfaceType.Name+" interface");
            property.objectReferenceValue = null;
        }
        else
        {
            property.objectReferenceValue = component;
        }
    }

    private void HandleComponent (SerializedProperty property, Type interfaceType)
    {
        Component component = property.objectReferenceValue as Component;
        if (component != null && interfaceType.IsAssignableFrom(component.GetType()) == false)
            component = component.GetComponent(interfaceType);
        if (component == null)
        {
            Debug.LogError(component.gameObject.name+" does not contain component implemented "+interfaceType.Name+" interface");
            property.objectReferenceValue = null;
        }
        else
        {
            property.objectReferenceValue = component;
        }
    }

    private void HandleGeneric (SerializedProperty property, Type interfaceType)
    {
        Object propertyObject = property.objectReferenceValue;
        if (interfaceType.IsAssignableFrom(propertyObject.GetType()) == false)
        {
            Debug.LogError("Object does not implement "+interfaceType.Name);
            property.objectReferenceValue = null;
        }
    }
}