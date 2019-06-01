#region Head

// Author:            LinYuzhou
// Email:             

#endregion

using Sirenix.OdinInspector;
using Study.DotNet.System.Collections.Generic;
using UnityEngine;

namespace DraftBorad
{
    [SerializeField]
    public class MethedInvokeBorad 
    {
        [BoxGroup("数据结构方法测试")]
        [Button]
        public void TestStackForeach()
        {
            Stack<int> stack = new Stack<int>();
            stack.Push(1);
            stack.Push(3);
            foreach(var i in stack)
            {
                Debug.Log(i);
            }
        }
    }
}

