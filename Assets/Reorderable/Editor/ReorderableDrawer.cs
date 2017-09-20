using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class ReorderableDrawer : PropertyDrawer
{
    // --------------------------------------------------------------------------------------------
    protected enum CollectionType
    {
        Array,
        List
    }

    // --------------------------------------------------------------------------------------------
    protected class ArrayFieldInfo
    {
        public SerializedProperty property;
        public object arrayOwner;
        public CollectionType collectionType;
        public FieldInfo arrayFieldInfo;
        public Type elementType;
        public int elementIndex;
        public bool isElementSimpleType;
    }

    // --------------------------------------------------------------------------------------------
    protected float m_buttonWidth = 18;
    protected float m_buttonHeight = 15;
    protected bool m_showAdd = true;
    protected bool m_showDelete = true;
    protected bool m_showOrder = true;
    protected bool m_showBox = true;
    private Color m_buttonsDarkSkinColor = new Color(0.7f, 0.7f, 0.7f, 0.6f);
    private Color m_buttonsLightSkinColor = new Color(0.7f, 0.7f, 0.7f, 0.4f);
    protected ArrayFieldInfo m_info = null;

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override to define the array header height
    /// </summary>
    /// <param name="property"></param>
    /// <param name="label"></param>
    /// <returns>The header height</returns>
    protected virtual float GetHeaderHeight(SerializedProperty property, GUIContent label)
    {
        return 0;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override to draw the array header
    /// </summary>
    /// <param name="property"></param>
    /// <param name="label"></param>
    /// <returns></returns>
    protected virtual void DrawHeader(Rect propertyRect, Rect headerRect, SerializedProperty property, GUIContent label)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override to define an array element height
    /// </summary>
    /// <param name="property"></param>
    /// <param name="label"></param>
    /// <returns>The element height</returns>
    protected virtual float GetElementHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override to draw an array element
    /// </summary>
    /// <param name="property"></param>
    /// <param name="label"></param>
    /// <returns></returns>
    protected virtual void DrawElement(Rect propertyRect, Rect elementRect, SerializedProperty property, GUIContent label)
    {
        var rect = m_info != null && m_info.isElementSimpleType ? elementRect : propertyRect;
        EditorGUI.PropertyField(rect, property, label, true);
    }

    // --------------------------------------------------------------------------------------------
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        var content = EditorGUI.BeginProperty(rect, label, property);

        m_info = GetArrayFieldInfo(property);

        var headerHeight = GetHeaderHeight(property, content);

        var elementRect = rect;
        elementRect.y += (m_info != null && m_info.elementIndex == 0) ? headerHeight : 0;

        if (m_info != null)
        {
            // Draw reorderable buttons first because we want to know their size, so the drawHeader 
            // method can properly adjust the position of its content.
            elementRect = DrawReorderableButtons(elementRect, property);
        }

        // Draw the header if it's the first element.
        if (m_info != null && m_info.elementIndex == 0)
        {
            var headerRect = new Rect(elementRect.x, elementRect.y - headerHeight, elementRect.width, headerHeight);
            DrawHeader(rect, headerRect, property, content);
        }

        elementRect.height = GetElementHeight(property, content);
        DrawElement(rect, elementRect, property, content);

        EditorGUI.EndProperty();
    }

    // --------------------------------------------------------------------------------------------
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        m_info = GetArrayFieldInfo(property);
        var height = GetElementHeight(property, label);
        if (m_info != null && m_info.elementIndex == 0)
        {
            height += GetHeaderHeight(property, label);
        }

        return height;
    }

    // --------------------------------------------------------------------------------------------
    protected Rect DrawReorderableButtons(Rect rect, SerializedProperty property)
    {
        var buttonCount = 0;
        buttonCount += (m_showOrder) ? 2 : 0;
        buttonCount += (m_showAdd) ? 1 : 0;
        buttonCount += (m_showDelete) ? 1 : 0;
        var buttonIndex = 0;

        GUI.backgroundColor = EditorGUIUtility.isProSkin ? m_buttonsDarkSkinColor : m_buttonsLightSkinColor;
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
                ReorderElement(m_info, isCtrlPressed ? int.MinValue : -1);
            }
            buttonIndex++;
            buttonRect.x += m_buttonWidth;

            if (GUI.Button(buttonRect, new GUIContent("\u25BC", "Ctrl+Click: move to bottom"), GetButtonStyle(buttonIndex, buttonCount)))
            {
                Undo.RecordObject(property.serializedObject.targetObject, "Reorder Element");
                ReorderElement(m_info, isCtrlPressed ? int.MaxValue : 1);
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
                InsertElement(m_info);
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
                DeleteElement(m_info);
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
    private static void ReorderElement(ArrayFieldInfo info, int offset)
    {
        var value = info.arrayFieldInfo.GetValue(info.arrayOwner);

        if (info.collectionType == CollectionType.Array)
        {
            var array = value as Array;
            var newIndex = Mathf.Clamp(info.elementIndex + offset, 0, array.Length - 1);
            var element = array.GetValue(info.elementIndex);

            // We don't simply swap the elements because the offset is not always 1 or -1
            // This function is also used to put elements on top or to the bottom.
            array = ArrayRemove(info.elementType, array, info.elementIndex);
            array = ArrayInsert(info.elementType, array, newIndex, element);
            info.arrayFieldInfo.SetValue(info.arrayOwner, array);
        }
        else if (info.collectionType == CollectionType.List)
        {
            var list = value as IList;
            var newIndex = Mathf.Clamp(info.elementIndex + offset, 0, list.Count - 1);
            var element = list[info.elementIndex];
            list.RemoveAt(info.elementIndex);
            list.Insert(newIndex, element);
        }
    }

    // --------------------------------------------------------------------------------------------
    private static void InsertElement(ArrayFieldInfo info)
    {
        var value = info.arrayFieldInfo.GetValue(info.arrayOwner);

        if (info.collectionType == CollectionType.Array)
        {
            var newInstance = Activator.CreateInstance(info.elementType);
            var array = ArrayInsert(info.elementType, value as Array, info.elementIndex + 1, newInstance);
            info.arrayFieldInfo.SetValue(info.arrayOwner, array);
        }
        else if (info.collectionType == CollectionType.List)
        {
            var newInstance = Activator.CreateInstance(info.elementType);
            var list = value as IList;
            list.Insert(info.elementIndex + 1, newInstance);
        }
    }

    // --------------------------------------------------------------------------------------------
    private static void DeleteElement(ArrayFieldInfo info)
    {
        var value = info.arrayFieldInfo.GetValue(info.arrayOwner);

        if (info.collectionType == CollectionType.Array)
        {
            var array = ArrayRemove(info.elementType, value as Array, info.elementIndex);
            info.arrayFieldInfo.SetValue(info.arrayOwner, array);
        }
        else if (info.collectionType == CollectionType.List)
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
    protected static ArrayFieldInfo GetArrayFieldInfo(SerializedProperty property)
    {
        // The property path should be something like : "myField.mySubField.myArray.Array.data[3]"
        var arrayPrefix = "Array.data[";

        var arrayPrefixIndex = property.propertyPath.LastIndexOf(arrayPrefix);
        if (arrayPrefixIndex < 0)
            return null;

        // Find 3 in "myField.mySubField.myArray.Array.data[3]"
        var elementIndexStr = property.propertyPath.Substring(arrayPrefixIndex + arrayPrefix.Length, property.propertyPath.Length - arrayPrefixIndex - arrayPrefix.Length - 1);
        var elementIndex = -1;
        if (int.TryParse(elementIndexStr, out elementIndex) == false)
            return null;

        // Run through the subField fields since the array might be inside multiple sub classes
        var fieldPath = property.propertyPath.Substring(0, arrayPrefixIndex - 1);
        var paths = fieldPath.Split('.');

        object instance = property.serializedObject.targetObject;
        var type = instance.GetType();

        FieldInfo field = null;

        for (var i = 0; i < paths.Length; ++i)
        {
            var fieldName = paths[i];
            bool isArray = instance is Array;
            bool isList = field != null && field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>);
            if (isArray || isList)
            {
                fieldName = paths[++i];
                var arrayIndexPrefix = fieldName.IndexOf("[");
                var arrayIndexStr = fieldName.Substring(arrayIndexPrefix + 1, fieldName.Length - (arrayIndexPrefix + 1) - 1);
                var arrayIndex = -1;
                if (int.TryParse(arrayIndexStr, out arrayIndex) == false)
                    return null;

                if (isArray)
                {
                    fieldName = paths[++i];
                    type = field.FieldType.GetElementType();
                    instance = (instance as Array).GetValue(arrayIndex);
                }
                else if (isList)
                {
                    fieldName = paths[++i];
                    type = field.FieldType.GetGenericArguments()[0];
                    instance = (instance as IList)[arrayIndex];
                }
            }

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
        CollectionType collectionType;
        Type elementType;

        if (field.FieldType.GetElementType() != null)
        {
            collectionType = CollectionType.Array;
            elementType = field.FieldType.GetElementType();
        }
        else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
        {
            collectionType = CollectionType.List;
            elementType = field.FieldType.GetGenericArguments()[0];
        }
        else
        {
            return null;
        }

        return new ArrayFieldInfo
        {
            property = property,
            arrayOwner = instance,
            collectionType = collectionType,
            arrayFieldInfo = field,
            elementType = elementType,
            elementIndex = elementIndex,
            isElementSimpleType = IsSimpleType(elementType),
        };
    }

    // --------------------------------------------------------------------------------------------
    private static bool IsSimpleType(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            // nullable type, check if the nested type is simple.
            return IsSimpleType(type.GetGenericArguments()[0]);
        }
        return type.IsPrimitive
            || type.IsEnum
            || typeof(UnityEngine.Object).IsAssignableFrom(type)
            || type.Equals(typeof(string))
            || type.Equals(typeof(decimal));
    }
}
