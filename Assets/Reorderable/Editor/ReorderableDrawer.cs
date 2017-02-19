using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class ReorderableDrawer : PropertyDrawer
{
    // --------------------------------------------------------------------------------------------
    private class ArrayFieldInfo
    {
        public object arrayOwner;
        public FieldInfo arrayFieldInfo;
        public int elementIndex;
    }

    // --------------------------------------------------------------------------------------------
    protected float m_buttonWidth = 18;
    protected float m_buttonHeight = 15;
    protected bool m_showAdd = true;
    protected bool m_showDelete = true;
    protected bool m_showOrder = true;
    protected bool m_showBox = true;
    private Color m_buttonsColor = new Color(0.7f, 0.7f, 0.7f, 0.4f);

    // --------------------------------------------------------------------------------------------
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(rect, label, property);
        var headerRect = DrawReorderableButtons(rect, property);
        DrawProperty(rect, headerRect, property, label);
        EditorGUI.EndProperty();
    }

    // --------------------------------------------------------------------------------------------
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    // --------------------------------------------------------------------------------------------
    protected virtual void DrawProperty(Rect propertyRect, Rect headerRect, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(propertyRect, property, label, true);
    }

    // --------------------------------------------------------------------------------------------
    protected Rect DrawReorderableButtons(Rect rect, SerializedProperty property)
    {
        var info = GetArrayFieldInfo(property);
        if (info == null)
            return rect;

        var buttonCount = 0;
        buttonCount += (m_showOrder) ? 2 : 0;
        buttonCount += (m_showAdd) ? 1 : 0;
        buttonCount += (m_showDelete) ? 1 : 0;
        var buttonIndex = 0;

        GUI.backgroundColor = m_buttonsColor;
        var buttonRect = new Rect(rect.x + rect.width - m_buttonWidth * buttonCount, rect.y, m_buttonWidth, m_buttonHeight);

        // ----------------------------------------------------------------------------------------
        // Reorder Buttons
        // ----------------------------------------------------------------------------------------
        if (m_showOrder)
        {
            var isCtrlPressed = (Event.current != null && Event.current.control);

            if (GUI.Button(buttonRect, new GUIContent("\u25B2", "Ctrl+Click: move to top"), GetButtonStyle(buttonIndex, buttonCount)))
            {
                Undo.RecordObject(property.serializedObject.targetObject, "Reorder Element");
                ReorderElement(property, isCtrlPressed ? int.MinValue : -1);
            }
            buttonIndex++;
            buttonRect.x += m_buttonWidth;

            if (GUI.Button(buttonRect, new GUIContent("\u25BC", "Ctrl+Click: move to bottom"), GetButtonStyle(buttonIndex, buttonCount)))
            {
                Undo.RecordObject(property.serializedObject.targetObject, "Reorder Element");
                ReorderElement(property, isCtrlPressed ? int.MaxValue : 1);
            }
            buttonIndex++;
            buttonRect.x += m_buttonWidth;
        }

        // ----------------------------------------------------------------------------------------
        // Add Element Button
        // ----------------------------------------------------------------------------------------
        if (m_showAdd)
        {
            if (GUI.Button(buttonRect, new GUIContent("+"), GetButtonStyle(buttonIndex, buttonCount)))
            {
                Undo.RecordObject(property.serializedObject.targetObject, "Add Element");
                InsertElement(property, info);
            }
            buttonRect.x += m_buttonWidth;
            buttonIndex++;
        }

        // ----------------------------------------------------------------------------------------
        // Delete Element Button
        // ----------------------------------------------------------------------------------------
        if (m_showDelete)
        {
            if (GUI.Button(buttonRect, new GUIContent("x"), GetButtonStyle(buttonIndex, buttonCount)))
            {
                Undo.RecordObject(property.serializedObject.targetObject, "Delete Element");
                DeleteElement(property, info);
            }
            buttonRect.x += m_buttonWidth;
            buttonIndex++;
        }

        GUI.backgroundColor = Color.white;

        rect.width -= m_buttonWidth * buttonCount;
        return rect;
    }

    // --------------------------------------------------------------------------------------------
    private GUIStyle GetButtonStyle(int buttonIndex, int buttonCount)
    {
        if (buttonIndex == buttonCount - 1)
            return EditorStyles.miniButtonRight;

        if (buttonIndex == 0)
            return EditorStyles.miniButtonLeft;

        return EditorStyles.miniButtonMid;
    }

    // --------------------------------------------------------------------------------------------
    public static void ReorderElement(SerializedProperty property, int offset)
    {
        var info = GetArrayFieldInfo(property);
        var value = info.arrayFieldInfo.GetValue(info.arrayOwner);
        var fieldType = info.arrayFieldInfo.FieldType;

        if (value is Array)
        {
            var array = value as Array;
            var newIndex = Mathf.Clamp(info.elementIndex + offset, 0, array.Length - 1);
            var element = array.GetValue(info.elementIndex);
            var elementType = fieldType.GetElementType();

            // We don't simply swap the elements because the offset is not always 1 or -1
            // This function is also used to put elements on top or to the bottom.
            array = ArrayRemove(elementType, array, info.elementIndex);
            array = ArrayInsert(elementType, array, newIndex, element);
            info.arrayFieldInfo.SetValue(info.arrayOwner, array);
        }
        else if (value is IList)
        {
            var list = value as IList;
            var newIndex = Mathf.Clamp(info.elementIndex + offset, 0, list.Count - 1);
            var element = list[info.elementIndex];
            list.RemoveAt(info.elementIndex);
            list.Insert(newIndex, element);
        }
    }

    // --------------------------------------------------------------------------------------------
    private static void InsertElement(SerializedProperty property, ArrayFieldInfo info)
    {
        var value = info.arrayFieldInfo.GetValue(info.arrayOwner);
        var fieldType = info.arrayFieldInfo.FieldType;

        if (value is Array)
        {
            var elementType = fieldType.GetElementType();
            var newInstance = Activator.CreateInstance(elementType);
            var array = value as Array;
            array = ArrayInsert(elementType, array, info.elementIndex + 1, newInstance);
            info.arrayFieldInfo.SetValue(info.arrayOwner, array);
        }
        else if (value is IList)
        {
            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var elementType = fieldType.GetGenericArguments()[0];
                var newInstance = Activator.CreateInstance(elementType);
                var list = value as IList;
                list.Insert(info.elementIndex + 1, newInstance);
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    private static void DeleteElement(SerializedProperty property, ArrayFieldInfo info)
    {
        var value = info.arrayFieldInfo.GetValue(info.arrayOwner);

        if (value is Array)
        {
            var array = value as Array;
            array = ArrayRemove(info.arrayFieldInfo.FieldType.GetElementType(), array, info.elementIndex);
            info.arrayFieldInfo.SetValue(info.arrayOwner, array);
        }
        else if (value is IList)
        {
            var list = value as IList;
            list.RemoveAt(info.elementIndex);
        }
    }

    // --------------------------------------------------------------------------------------------
    private static Array ArrayInsert(Type arrayFieldType, Array source, int index, object element)
    {
        var newArray = Array.CreateInstance(arrayFieldType, source.Length + 1);
        if (index > 0)
        {
            Array.Copy(source, 0, newArray, 0, index);
        }

        newArray.SetValue(element, index);

        if (index < source.Length)
        {
            Array.Copy(source, index, newArray, index + 1, source.Length - index);
        }
        return newArray;
    }

    // --------------------------------------------------------------------------------------------
    private static Array ArrayRemove(Type arrayFieldType, Array source, int index)
    {
        var dest = Array.CreateInstance(arrayFieldType, source.Length - 1);

        if (index > 0)
        {
            Array.Copy(source, 0, dest, 0, index);
        }

        if (index < source.Length - 1)
        {
            Array.Copy(source, index + 1, dest, index, source.Length - index - 1);
        }

        return dest;
    }

    // --------------------------------------------------------------------------------------------
    private static ArrayFieldInfo GetArrayFieldInfo(SerializedProperty property)
    {
        // The property path should be somthing like : "myField.mySubField.myArray.Array.data[3]"

        var arrayPrefix = "Array.data[";
        var arrayPrefixIndex = property.propertyPath.IndexOf(arrayPrefix);
        var elementIndexStr = property.propertyPath.Substring(arrayPrefixIndex + arrayPrefix.Length, property.propertyPath.Length - arrayPrefixIndex - arrayPrefix.Length - 1);
        var elementIndex = -1;
        if (int.TryParse(elementIndexStr, out elementIndex) == false)
            return null;

        // Run through the subField fields since the array might be inside multiple sub classes
        var fieldPath = property.propertyPath.Substring(0, arrayPrefixIndex - 1);
        var paths = fieldPath.Split('.');
        var type = property.serializedObject.targetObject.GetType();

        object instance = property.serializedObject.targetObject;
        FieldInfo field = null;

        for (var i = 0; i < paths.Length; ++i)
        {
            var fieldName = paths[i];

            // Iterate over the base type because GetField returns null if the field is private inside a base class.
            var baseType = type;
            while (baseType != null)
            {
                field = baseType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                    break;

                baseType = baseType.BaseType;
            }

            if (field == null)
                return null;

            if (i != paths.Length - 1)
            {
                instance = field.GetValue(instance);
                type = field.FieldType;
            }
        }

        // We only support List and Arrays
        var value = field.GetValue(instance);
        if ((value is Array || value is IList) == false)
            return null;

        return new ArrayFieldInfo { arrayOwner = instance, arrayFieldInfo = field, elementIndex = elementIndex };
    }
}
