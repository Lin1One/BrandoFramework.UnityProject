#region Head

// Author:            LinYuzhou
// Email:             

#endregion

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace DraftBorad
{
    [SerializeField]
    public class MethedInvokeBorad 
    {
        [BoxGroup("数据结构方法测试")]
        [Button]
        public void TestStackForeach()
        {
            //Stack<int> stack = new Stack<int>();
            //stack.Push(1);
            //stack.Push(3);
            //foreach(var i in stack)
            //{
            //    Debug.Log(i);
            //}
        }

        [BoxGroup("UI 层次测试")]
        [Button]
        public void LogUIControlDepth()
        {
            var canvas = GameObject.Find("DynamicCanvas");
            canvas.GetComponent<Canvas>().overrideSorting = true;
            var uiGraphic = canvas.GetComponentsInChildren<Graphic>();
            foreach (var i in uiGraphic)
            {
                Debug.Log(i.gameObject.name + "  " + i.depth);
            }
            Debug.Log(canvas.GetComponent<Canvas>().overrideSorting);
        }
    }
}

