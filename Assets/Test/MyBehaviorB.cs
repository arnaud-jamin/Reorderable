using System;
using UnityEngine;

[SelectionBase]
public class MyBehaviorB : MyBehaviorA
{
    [Serializable]
    public class MyItemC
    {
        public string name;
    }

    [Serializable]
    public class MyItemB : MyItemA
    {
        [Range(0, 1)]
        public float time;

        [Reorderable]
        public MyItemC[] arrayInArray;
    }

    [Reorderable]
    [SerializeField]
    public MyItemB[] anotherArrayWithAttribute = null;
}
