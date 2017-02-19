using System;
using UnityEngine;

[SelectionBase]
public class MyBehaviorB : MyBehaviorA
{
    [Serializable]
    public class MyItemB : MyItemA
    {
        [Range(0, 1)]
        [SerializeField]
        private float m_time;
    }

    [Reorderable]
    [SerializeField]
    private MyItemB[] anotherArrayWithAttribute = null;

    public MyItemB[] AnotherArrayWithAttribute { get { return anotherArrayWithAttribute; } } // to remove warnigs
}
