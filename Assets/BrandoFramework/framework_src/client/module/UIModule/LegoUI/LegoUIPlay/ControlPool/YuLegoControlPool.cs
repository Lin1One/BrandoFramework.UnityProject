#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using Client.Extend;
using Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高控件池。
    /// </summary>
    [YuMonoPath("UI/ControlPoolCanvas/", null)]
    [Singleton]
    public class YuLegoControlPool<T> :
        MonoBehaviour
        where T : Component
    {
        #region 基础字段及属性

        private string controlType;
        private int initCount;

        [LabelText("控件池")] [ShowInInspector] private T[] controls;
        private int poolMaxIndex;
        private int poolLength;
        private int useIndex = -1;
        private YuU3dAppLegoUISetting legoSetting;

        #endregion

        #region 剩余数量

#if DEBUG
        [LabelText("空闲数量")] [ShowInInspector] private int unuseCount;

#endif

        #endregion

        #region 控件预制体

        private GameObject template;

        private GameObject Template
        {
            get
            {
                if (template != null)
                {
                    return template;
                }

                template = LegoDefaultControls.GetControl<T>();

                var templateRootGo = transform.parent
                    .Find("Template");

                if (templateRootGo == null)
                {
                    templateRootGo = transform.parent.AddChild("Template");
                }

                template.transform.SetParent(templateRootGo);
                return template;
            }
        }

        #endregion

        #region 依赖注入及初始化

        [Button("AutoInject")]
        protected void AutoInject()
        {
            // 设置自身layer为ControlPoolCanvas的layer
            gameObject.layer = transform.parent.gameObject.layer;
            InitSetting();
            InitPool();
        }

        private void InitSetting()
        {
            //var typeName = typeof(T).Name;
            //controlType = typeName.StartsWith("YuLego")
            //    ? typeName.Substring(6, typeName.Length - 6)
            //    : typeName;

            //legoSetting = YuU3dAppLegoUISettingDati.CurrentActual;
            //if (legoSetting == null)
            //{
            //    initCount = 5;
            //    Debug.Log("乐高UI控件池配置丢失，初始化数量被设置为5！");
            //    return;
            //}

            //var poolSetting = legoSetting.PoolSetting;
            //var fieldInfo = poolSetting.GetType().GetField(controlType + "PoolInitCount");
            //var pValue = fieldInfo.GetValue(poolSetting);
            //initCount = (int) pValue;
        }

        private void InitPool()
        {
            controls = new T[initCount];
            poolMaxIndex = initCount - 1;
            poolLength = initCount;
            // 修正池自身根对象的位置、缩放。
            var transform1 = transform;
            transform1.localPosition = Vector3.zero;
            transform1.localScale = Vector3.one;

            for (var i = 0; i < initCount; i++)
            {
                var go = CreateNewControl();
                var control = go.GetComponent<T>();
                //control.gameObject.SetActive(false);
                controls[i] = control;
            }

#if DEBUG
            unuseCount = poolLength - useIndex;
#endif
        }

        private GameObject CreateNewControl()
        {
            var go = Instantiate(Template, transform, true);
            go.name = Template.name;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.gameObject.layer = gameObject.layer;
            return go;
        }

        #endregion

        #region 存取控件

        public T Take()
        {
            // 耗尽
            if (useIndex == poolMaxIndex)
            {
                var oldPool = controls;
                var oldLength = controls.Length;
                //YuDebugUtility.Log($"类型{typeof(T)}的控件池当前容量为{oldLength}，已耗尽！");
                var newPool = new T[oldLength * 2];
                for (var i = 0; i < oldLength; i++)
                {
                    var e = oldPool[i];
                    newPool[i] = e;
                }

                // 填充新控件到新池

                for (var i = 0; i < oldLength; i++)
                {
                    var go = CreateNewControl();
                    newPool[oldLength + i] = go.GetComponent<T>();
                }

                //YuDebugUtility.Log($"类型{typeof(T)}的控件池初始化容量已经扩容为{newPool.Length}！");

                // 指向新建的池
                controls = newPool;
                poolMaxIndex = controls.Length - 1;
                poolLength = controls.Length;
            }

            useIndex++;

            // 编辑器下调试更新当前空闲数量
#if DEBUG
            unuseCount = poolLength - useIndex;
#endif

            return controls[useIndex];
        }

        public void Restore(T control)
        {
            //control.gameObject.SetActive(false);
            control.transform.SetParent(transform);
            controls[useIndex] = control;
            useIndex--;
        }

        #endregion
    }
}