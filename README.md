# Reorderable

In Unity 3D reorderable lists are usually prefered over the default array/list inspector for their ability to reorder elements using drag and drop. But to use reorderable lists, it is required to create a custom editor. It is not always convenient to do so, because it forces to manage all the others properties of the object through the custom editor. This tool lets you quickly get reorderable list by using a simple custom attribute instead of having to create a custom editor. 

By adding the [Reorderable] attribute ...
```cs
    [Reorderable]
    public MyItem[] myArray;
```

You get the following controls (Drag and drop is not supported):
 - Move Up
 - Move Down
 - Move Top (Ctrl+Click on Move Up)
 - Move Bottom (Ctrl+Click on Move Down)
 - Add Element 
 - Remove Element 

This is how the array looks when collpased:

![alt tag](https://cloud.githubusercontent.com/assets/13844285/23100455/1410a36e-f64f-11e6-8d53-19814474632f.png)

When open:

![alt tag](https://cloud.githubusercontent.com/assets/13844285/23100244/25737f14-f64a-11e6-8c43-9717b01ced71.png)

You can also create your own ReorderableDrawer to display a array header, and change the way each element are displayed. For example, the following image is drawn by the class below:

![alt tag](https://cloud.githubusercontent.com/assets/13844285/23328168/3c4640ca-fae9-11e6-8436-74d77124d032.png)

```cs
[CustomPropertyDrawer(typeof(MyItem))]
public class MyCustomDrawer : ReorderableDrawer
{
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        m_showAdd = true;
        m_showDelete = true;
        m_showOrder = true;
        m_showBox = true;
        base.OnGUI(rect, property, label);
    }

    protected override float GetHeaderHeight(SerializedProperty property, GUIContent label)
    {
        return 16;
    }

    protected override float GetElementHeight(SerializedProperty property, GUIContent label)
    {
        return 16;
    }

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
```

