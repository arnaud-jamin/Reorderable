using System;
using UnityEngine;

[SelectionBase]
public class MyBehaviorB : MyBehaviorA
{
    [Serializable]
    public class MyItemB : MyItemA
    {
        [Range(0, 1)]
        public float time;
    }

    [Serializable]
    public class MyInnerClass3
    {
        [Reorderable]
        public MyItemB[] myArray = null;
    }

    [Reorderable]
    public MyItemB[] publicArray3 = null;

    [Reorderable]
    [SerializeField]
    private MyItemB[] privateArray3 = null;

    public MyItemB[] PrivateArray3 { get { return privateArray3; } } // to remove warnigs
}
