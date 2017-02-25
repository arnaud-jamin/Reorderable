using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MyBehaviorA.MyItemA))]
public class MyCustomDrawer : ReorderableDrawer
{
    // --------------------------------------------------------------------------------------------
    static int s_headerHeight = 16;
    static int s_propertyHeight = 16;

    // --------------------------------------------------------------------------------------------
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        m_showAdd = true;
        m_showDelete = true;
        m_showOrder = true;
        m_showBox = true;
        base.OnGUI(rect, property, label);
    }

    // ----------------------------------------------------------------------------------------
    protected override float GetHeaderHeight(SerializedProperty property, GUIContent label)
    {
        return s_headerHeight;
    }

    //-----------------------------------------------------------------------------------------
    protected override float GetElementHeight(SerializedProperty property, GUIContent label)
    {
        return s_propertyHeight;
    }

    //-----------------------------------------------------------------------------------------
    protected override void DrawHeader(Rect propertyRect, Rect headerRect, SerializedProperty property, GUIContent label)
    {
        headerRect.x += EditorGUIUtility.labelWidth;
        headerRect.width -= EditorGUIUtility.labelWidth;
        headerRect.width = headerRect.width / 3;

        GUI.color = Color.white;
        GUI.backgroundColor = Color.white;
        GUI.Label(headerRect, " Parent");

        headerRect.x += headerRect.width;
        GUI.Label(headerRect, " Child");

        headerRect.x += headerRect.width;
        GUI.Label(headerRect, " Padding");
    }

    // --------------------------------------------------------------------------------------------
    protected override void DrawElement(Rect propertyRect, Rect elementRect, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(elementRect, label, property);

        elementRect = EditorGUI.PrefixLabel(elementRect, GUIUtility.GetControlID(FocusType.Passive), label);
        elementRect.width = elementRect.width / 3.0f;

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        
        EditorGUI.PropertyField(elementRect, property.FindPropertyRelative("m_name"), GUIContent.none);
        elementRect.x += elementRect.width;

        EditorGUI.PropertyField(elementRect, property.FindPropertyRelative("m_value"), GUIContent.none);
        elementRect.x += elementRect.width;

        EditorGUI.PropertyField(elementRect, property.FindPropertyRelative("m_color"), GUIContent.none);

        EditorGUI.EndProperty();
        EditorGUI.indentLevel = indent;
    }
}
