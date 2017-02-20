# Reorderable

In Unity 3D reorderable list are prefered over the default array/list inspector. But to use reorderable list, it is required to create a custom editor. It is not always convenient to do so, because if forces to manage all the others properties of the object through the custom editor. This tool let you quickly get reorderable list by using a simple custom attribute, instead of having to create a custom editor. 

By adding the [Reorderable] attribute ...
```cs
    [Reorderable]
    public MyItem[] myArray;
```

You get the following controls:
 - Move Up
 - Move Down
 - Move Top (Ctrl+Click on Move Up)
 - Move Bottom (Ctrl+Click on Move Down)
 - Add Element 
 - Remove Element 

- **Collpased**:

![alt tag](https://cloud.githubusercontent.com/assets/13844285/23100455/1410a36e-f64f-11e6-8d53-19814474632f.png)

- **Open**:

![alt tag](https://cloud.githubusercontent.com/assets/13844285/23100244/25737f14-f64a-11e6-8c43-9717b01ced71.png)

You can also create your own ReorderableDrawer to change the way to display each element. For example, the following image is drawn by the class below:

![alt tag](https://cloud.githubusercontent.com/assets/13844285/23100293/77817832-f64b-11e6-8e69-dd9eb83118ec.png)

```cs
[CustomPropertyDrawer(typeof(MyItem))]
public class MyCustomDrawer : ReorderableDrawer
{
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        m_showAdd = true;
        m_showDelete = true;
        m_showOrder = true;
        base.OnGUI(rect, property, label);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 16;
    }

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
```

