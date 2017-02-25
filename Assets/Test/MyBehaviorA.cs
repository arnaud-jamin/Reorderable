using System;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class MyBehaviorA : MonoBehaviour
{
    [Serializable]
    public class MyItemA
    {
        [SerializeField]
        private string m_name = string.Empty;

        [SerializeField]
        private int m_value = 0;

        [SerializeField]
        private Color m_color = Color.white;

        public string Name { get { return m_name; } }
        public int Value { get { return m_value; } }
        public Color Color { get { return m_color; } }
    }

    [Serializable]
    public class MyInnerClass1
    {
        [SerializeField]
        private List<MyItemA> listInSubFields = null;

        public List<MyItemA> List { get { return listInSubFields; } }
    }

    [Serializable]
    public class MyInnerClass2
    {
        [SerializeField]
        private MyInnerClass1 m_anotherField = null;

        [SerializeField]
        private MyItemA[] m_arrayInAField = null;

        public MyInnerClass1 AnothField { get { return m_anotherField; } }
        public MyItemA[] ArrayInAField { get { return m_arrayInAField; } }
    }

    [SerializeField]
    [Reorderable]
    private MyItemA[] m_arrayWithAttribute = null;

    [SerializeField]
    private MyItemA[] m_arrayWithCustomDrawer = null;

    [SerializeField]
    private List<MyItemA> m_listWithCustomDrawer = null;

    [SerializeField]
    [Reorderable]
    private List<float> m_listOfSimpleType = null;

    [SerializeField]
    private MyInnerClass2 field = null;

    public MyItemA[] ArrayWithAttribute { get { return m_arrayWithAttribute; } }
    public MyItemA[] ArrayWithCustomDrawer { get { return m_arrayWithCustomDrawer; } }
    public List<MyItemA> ListWithCustomDrawer { get { return m_listWithCustomDrawer; } }
    public List<float> listOfSimpleType { get { return m_listOfSimpleType; } }
    public MyInnerClass2 PrivateField { get { return field; } }
}
