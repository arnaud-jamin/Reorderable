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

        public string Name { get { return m_name; } } // to remove warnigs
        public int Value { get { return m_value; } } // to remove warnigs
        public Color Color { get { return m_color; } } // to remove warnigs
    }

    [Serializable]
    public class MyInnerClass1
    {
        [SerializeField]
        private List<MyItemA> listInMultipleFields = null;

        public List<MyItemA> List { get { return listInMultipleFields; } } // to remove warnigs
    }

    [Serializable]
    public class MyInnerClass2
    {
        [SerializeField]
        private MyInnerClass1 m_anothField = null;

        [SerializeField]
        private MyItemA[] m_arrayInField = null;

        public MyInnerClass1 AnothField { get { return m_anothField; } } // to remove warnigs
        public MyItemA[] ArrayInField { get { return m_arrayInField; } } // to remove warnigs
    }

    [SerializeField]
    [Reorderable]
    private MyItemA[] m_arrayWithAttribute = null;

    [SerializeField]
    private MyItemA[] m_arrayWithCustomDrawer = null;

    [SerializeField]
    private List<MyItemA> m_listWithCustomDrawer = null;

    [SerializeField]
    private MyInnerClass2 field = null;

    public MyItemA[] ArrayWithAttribute { get { return m_arrayWithAttribute; } } // to remove warnigs
    public MyItemA[] ArrayWithCustomDrawer { get { return m_arrayWithCustomDrawer; } } // to remove warnigs
    public List<MyItemA> ListWithCustomDrawer { get { return m_listWithCustomDrawer; } } // to remove warnigs
    public MyInnerClass2 PrivateField { get { return field; } } // to re move warnigs
}
