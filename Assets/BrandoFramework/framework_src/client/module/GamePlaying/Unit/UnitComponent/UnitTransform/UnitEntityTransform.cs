#region Head

// Author:            Chengkefu
// CreateDate:        2018/11/22 14:18:15
// Email:             chengkefu0730@live.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Client.Assets;
using Common;
using Common.DataStruct;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.GamePlaying.Unit
{
    /// <summary>
    /// 角色的U3D数据类
    /// </summary>
    public class XTwoU3DData : UnitComponent , IUnitEntityTransform
    {
        private bool m_isActive;

        public bool Active
        {
            get
            {
                return m_isActive;
            }
        }

        private bool m_isCreated;       //是否已经完成资源加载
        private bool m_isRelease;       //是否已经被释放

        private int m_layer = -1;
        private string m_assetId;

        private GameObject m_assetRef;
        private Transform m_rootTansform;                 //根节点Transform

        //protected IYuMapLoadService m_mapLoadService;   //地图加载模块
        protected Transform m_roleRoot;
        public Transform Trans
        {
            get
            {
                return m_rootTansform;
            }
        }

        private readonly List<Material> m_listMainMaterial =
            new List<Material>();             //模型主要材质

        public List<Material> Materials
        {
            get { return m_listMainMaterial; }
        }

        //-----------Transform管理----------------
        protected Vector3 m_forward;    //朝向
        public Vector3 Forward { get { return m_forward; } }
        public Vector2 Forward2D { get { return new Vector2(m_forward.x, m_forward.z); } }

        protected float m_scale;
        public float Scale
        {
            get
            {
                return m_scale;
            }
            set
            {
                m_scale = value;
                if (m_rootTansform != null)
                {
                    m_rootTansform.localScale = Vector3.one * value;
                }
            }
        }
        protected Vector2 m_curPos;       //当前坐标位置
        protected Vector3 m_curPos3D;
        /// <summary>
        /// 当前坐标
        /// </summary>
        public Vector2 Position2D
        {
            get { return m_curPos; }
            set { m_curPos = value; }
        }
        /// <summary>
        /// 所在Unity世界坐标
        /// </summary>
        public Vector3 Position
        {
            get { return m_curPos3D; }
        }

        public Vector2 TargetPos { get; } = Vector2.zero;

        public Vector2 TargetDir { get; } = new Vector2(0,1);

        protected override void OnInit()
        {
            //if (m_mapLoadService == null)
            //{
            //    m_mapLoadService = YuU3dAppUtility.Injector.Get<IYuMapLoadService>();
            //}

            m_scale = 1.0f;
            m_isCreated = false;
            m_isRelease = false;
            m_isActive = true;
        }

        protected override void OnRelease()
        {
            m_assetId = null;
            m_isRelease = true;
            m_layer = -1;

            DestoryModule();
        }

        public void LoadAsset(string assetId, bool isSync, Action<UnitEntityBase> onCreated)
        {
            m_assetId = assetId;
            try
            {
                uint curNeedCount = RefCount;
                if (!isSync)    //异步加载方式
                {
                    Injector.Instance.Get<IAssetModule>().LoadAsync<GameObject>(assetId, (assetRef) =>
                    {
                        if (m_isRelease || curNeedCount != RefCount)  //如果被释放或是不属于当前引用，跳出
                        {
                            return;
                        }

                        if (assetRef != null)
                        {
                            m_isCreated = true;
                            InitUnityObj(assetRef);
                        }
                        else
                        {
                            //模型为空时，加载默认模型
                            assetRef = Injector.Instance.Get<IAssetModule>().Load<GameObject>("jiannv_model");
                            m_isCreated = true;
                            InitUnityObj(assetRef);
                        }
                        onCreated?.Invoke(Role);
                        ;
                    });
                }
                else        //同步加载方式
                {
                    var assetRef = Injector.Instance.Get<IAssetModule>().Load<GameObject>(assetId);
                    if (assetRef != null)
                    {
                        m_isCreated = true;
                        InitUnityObj(assetRef);
                    }
                    onCreated?.Invoke(Role);
                }
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.LogError(e);
#endif
            }
        }

        private Transform unitRoot;
        private Transform UnitRoot
        {
            get
            {
                if(unitRoot == null)
                {
                    unitRoot = GameObject.Find("UnitRoot").transform;
                }
                return unitRoot;
            }
        }
        

        public void BindGameObject(string GameObjectName)
        {
            var UnitGameObject = UnitRoot.Find(GameObjectName).gameObject;
            InitUnityObj(UnitGameObject);
        }

        private void DestoryModule()
        {
            m_isCreated = false;
            m_listMainMaterial.Clear();
            if (m_rootTansform != null)
            {
                GameObject.Destroy(m_rootTansform.gameObject);
                m_rootTansform = null;

                //Todo 释放引用
                //m_assetRef.Releas
            }

        }

        //获取资源引用成功后，实例化gameobject，初始化相关信息
        private void InitUnityObj(GameObject gameObject)
        {
            m_assetRef = gameObject;
            m_rootTansform = GameObject.Instantiate(gameObject).transform;
            //if (m_roleRoot == null)
            //{
            //    m_roleRoot = Injector.Instance.Get<UnitModule>().RoleRoot;
            //}
            m_rootTansform.SetParent(UnitRoot);
            foreach (var render in m_rootTansform.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                foreach (var mat in render.materials)
                {
                    //Todo 是否改为使用renderer统一配置mat数据的方式
                    m_listMainMaterial.Add(mat);
                }
            }
            m_rootTansform.localScale = Vector3.one * m_scale;

            if(m_layer >-1)
            {
                SetLayer(m_layer);
            }
        }

        public void SetActive(bool active)
        {
            if (m_isActive == active)
            {
                return;
            }
            m_isActive = active;

            if (m_rootTansform != null)
            {
                m_rootTansform.gameObject.SetActive(active);
            }
            //Role.Mount?.SetActive(active);
            //Role.PendantManager?.SetActive(active);

            Role.AnimaControl?.UnitPlayAnima("idle", true);
        }

        /// <summary>
        /// 重新加载模型
        /// </summary>
        /// <param name="assetId"></param>
        /// <param name="isSync"></param>
        /// <param name="onCreated"></param>
        public void ResetModule(string assetId, bool isSync, Action<UnitEntityBase> onCreated)
        {
            //Role.Mount?.GetOnMount(false);
            //Role.PendantManager?.StripAllPendant(true);
            DestoryModule();

            if (onCreated == null)
            {
                onCreated = (unit) =>
                {
                    //Role.PendantManager?.StripAllPendant(false);
                    RefreshTrans();
                };
            }
            else
            {
                onCreated += (unit) =>
                {
                    //Role.PendantManager?.StripAllPendant(false);
                    RefreshTrans();
                };
            }

            LoadAsset(assetId, isSync, onCreated);
        }

        /// <summary>
        /// 设置角色的显示layer
        /// </summary>
        /// <param name="index"></param>
        public void SetLayer(int index)
        {
            m_layer = index;
            if (Trans == null)
            {
                return;
            }

            Transform[] transArr = Trans.GetComponentsInChildren<Transform>();
            foreach (var trans in transArr)
            {
                trans.gameObject.layer = index;
            }
        }

        /// <summary>
        /// 刷新Transform，与记录的数据同步
        /// </summary>
        public virtual void RefreshTrans()
        {
            if (Trans == null)
            {
                return;
            }
            Trans.position = m_curPos3D;
            if (m_forward != Vector3.zero)
            {
                Trans.forward = m_forward;
            }
        }

        /// <summary>
        /// 设置到服务器二维坐标位置
        /// </summary>
        /// <param name="pos"></param>
        public void SetCoord(Point2 coord)
        {
            //SetPostion(YuGraphAlgorithm.GetPositionByCoord(
            //    coord, m_mapLoadService.OriPos));
        }

        /// <summary>
        /// 强行设置位置3D
        /// </summary>
        /// <param name="pos"></param>
        public virtual void SetPostion(Vector3 pos)
        {
            m_curPos = new Vector2(pos.x, pos.z);
            m_curPos3D = pos;
            if (Trans != null)
            {
                Trans.position = m_curPos3D;
            }
        }

        /// <summary>
        /// 强行设置位置，如果位置不再可行走区域，则找最近可行走点
        /// </summary>
        /// <param name="pos"></param>
        public virtual void SetPostion(Vector2 pos)
        {
            m_curPos = pos;
            m_curPos3D = GetHeightByPos2D(pos);
            if (Trans != null)
            {
                Trans.position = m_curPos3D;
            }
        }

        //通过2d坐标获取到y值
        protected Vector3 GetHeightByPos2D(Vector2 pos)
        {
            //float height = m_mapLoadService.GetHeight(pos);

            return new Vector3(pos.x, 0, pos.y);
            
        }

        /// <summary>
        /// 设置角色朝向
        /// </summary>
        /// <param name="dir"></param>
        public virtual void SetDirect(Vector2 dir)
        {
            m_forward = new Vector3(dir.x, 0.0f, dir.y);
            if (Trans != null)
            {
                Trans.forward = m_forward;
            }
        }

        /// <summary>
        /// 设置欧拉角旋转量
        /// </summary>
        /// <param name="dir"></param>
        public virtual void SetEulerAngle(Vector3 angle)
        {
            Quaternion rot = Quaternion.Euler(angle);
            m_forward = rot * Vector3.forward;
            if (Trans != null)
            {
                Trans.forward = m_forward;
            }
        }
    }
}