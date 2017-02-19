using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ReorderableAttribute))]
public class ReorderableAttributeDrawer : ReorderableDrawer
{
    // ----------------------------------------------------------------------------------------
    private float m_indentationWidth = 15;              // Guess of the width of one Unity indentation (EditorGUI.indentLevel)
    private float m_closedLineMaxHeight = 20;           // Threshold used to know if the property is opened
    private float m_verticalSpacingWhenOpened = 10;     // Spacing after an opened element
    private float m_verticalSpacingWhenClosed = 0;      // Spacing after a closed element 
    private float m_bottomPaddingWhenOpened = 10;       // Bottom padding of the box when opened
    private Color m_boxColor = new Color(0.6f, 0.6f, 0.6f, 0.3f);

    // ----------------------------------------------------------------------------------------
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = EditorGUI.GetPropertyHeight(property, label, true);
        height += (height > m_closedLineMaxHeight) ? m_bottomPaddingWhenOpened + m_verticalSpacingWhenOpened : m_verticalSpacingWhenClosed;
        return height;
    }

    // ----------------------------------------------------------------------------------------
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        var reorderable = attribute as ReorderableAttribute;
        m_showAdd = reorderable.showAdd;
        m_showDelete = reorderable.showDelete;
        m_showOrder = reorderable.showOrder;
        m_showBox = reorderable.showBox;

        EditorGUI.BeginProperty(rect, label, property);

        if (m_showBox)
        {
            var boxRect = rect;
            var elementFoldoutX = (EditorGUI.indentLevel - 1) * m_indentationWidth;
            boxRect.x += elementFoldoutX;
            boxRect.width -= elementFoldoutX; 
            boxRect.height -= (rect.height > m_closedLineMaxHeight) ? m_verticalSpacingWhenOpened : m_verticalSpacingWhenClosed;
            GUI.backgroundColor = m_boxColor;
            GUI.Box(boxRect, string.Empty);
            GUI.backgroundColor = Color.white;
        }

        var headerRect = DrawReorderableButtons(rect, property);
        DrawProperty(rect, headerRect, property, label);
        EditorGUI.EndProperty();
    }
}
