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
        // Test a public list in field in a field
        public List<MyItemA> publicList = null;

        // Test a private list in field in a field
        [SerializeField]
        private List<MyItemA> privateList = null;

        public List<MyItemA> PrivateList { get { return privateList; } } // to remove warnigs
    }

    [Serializable]
    public class MyInnerClass2
    {
        // Test a field in a field
        [SerializeField]
        private MyInnerClass1 privateField = null;

        // Test a public array in a base class
        public MyItemA[] publicArrayInField = null;

        // Test a public array in a base class
        [SerializeField]
        private MyItemA[] privateArrayInField = null;

        public MyInnerClass1 PrivateField { get { return privateField; } } // to remove warnigs
        public MyItemA[] PrivateArrayInField { get { return privateArrayInField; } } // to remove warnigs
    }

    public MyItemA[] publicArrayWithCustomDrawer = null;

    // Test a private array in a base class
    [SerializeField]
    private MyItemA[] privateArrayWithCustomDrawer = null;

    // Test a public array in a base class with attribute
    [Reorderable]
    public MyItemA[] publicArrayWithAttribute = null;

    // Test a private array in a base class with attribute
    [SerializeField]
    [Reorderable]
    private MyItemA[] privateArrayWithAttribute = null;

    // Test a public field in a base class
    public MyInnerClass2 publicField;

    // Test a private field in a base class
    [SerializeField]
    private MyInnerClass2 privateField = null;

    public MyItemA[] PrivateArray { get { return privateArrayWithCustomDrawer; } } // to remove warnigs
    public MyItemA[] PrivateArray2 { get { return privateArrayWithAttribute; } } // to remove warnigs
    public MyInnerClass2 PrivateField { get { return privateField; } } // to remove warnigs
}
