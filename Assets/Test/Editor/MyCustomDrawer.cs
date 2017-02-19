using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MyBehaviorA.MyItemA))]
public class MyCustomDrawer : ReorderableDrawer
{
    // --------------------------------------------------------------------------------------------
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        m_showAdd = true;
        m_showDelete = true;
        m_showOrder = true;
        m_showBox = true;
        base.OnGUI(rect, property, label);
    }

    // --------------------------------------------------------------------------------------------
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 16;
    }

    // --------------------------------------------------------------------------------------------
    protected override void DrawProperty(Rect propertyRect, Rect headerRect, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(propertyRect, label, property);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        headerRect.width = (headerRect.width - EditorGUIUtility.labelWidth - 20) / 3;
        headerRect.x += EditorGUIUtility.labelWidth;
        EditorGUI.PropertyField(headerRect, property.FindPropertyRelative("m_name"), GUIContent.none);

        headerRect.x += headerRect.width;
        EditorGUI.PropertyField(headerRect, property.FindPropertyRelative("m_value"), GUIContent.none);

        headerRect.x += headerRect.width;
        EditorGUI.PropertyField(headerRect, property.FindPropertyRelative("m_color"), GUIContent.none);

        EditorGUI.EndProperty();
        EditorGUI.indentLevel = indent;
    }
}