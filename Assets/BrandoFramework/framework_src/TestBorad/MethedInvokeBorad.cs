#region Head

// Author:            LinYuzhou
// Email:             

#endregion

using Client.DataTable.Editor;
using Sirenix.OdinInspector;
using Study.DotNet.System.Collections.Generic;
using System.Collections.Generic;
using TestProject;
using UnityEngine;
using UnityEngine.UI;

namespace DraftBorad
{
    [SerializeField]
    public class MethedInvokeBorad 
    {
        [BoxGroup("���ݽṹ��������")]
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

        [BoxGroup("UI ��β���")]
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

        //[LabelText("11")]
        //public TableDataBoard<TestProject_ExcelEntity_Test> datas;

        [LabelText("22")]
        [TableList]
        public List<TestProject_ExcelEntity_Test> data;
    }
}

