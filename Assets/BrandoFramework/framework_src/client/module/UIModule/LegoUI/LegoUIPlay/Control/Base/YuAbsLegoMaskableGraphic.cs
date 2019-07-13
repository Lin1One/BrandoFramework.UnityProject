#region Head

// Author:            Yu
// CreateDate:        2018/8/15 20:57:33
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Client.Assets;
using Common;
using UnityEngine;
using UnityEngine.UI;
using YuCommon;
using YuU3dPlay;

namespace Client.LegoUI
{
    public abstract class YuAbsLegoMaskableGraphic :
        MaskableGraphic,
        ILegoControl,
        IRelease
    {
        public string Name => name;
        private RectTransform m_RectTransform;
        protected static IAssetModule assetModule;

        protected static IAssetModule AssetModule
        {
            get
            {
                if (assetModule != null)
                {
                    return assetModule;
                }

                assetModule = Injector.Instance.Get<IAssetModule>();
                return assetModule;
            }
        }

        public ILegoUI LocUI { get; private set; }

        public LegoRectTransformMeta RectMeta { get; protected set; }

        public RectTransform RectTransform
        {
            get
            {
                if (m_RectTransform != null)
                {
                    return m_RectTransform;
                }

                m_RectTransform = GetComponent<RectTransform>();
                return m_RectTransform;
            }
        }

        private GameObject selfGo;

        public GameObject GameObject
        {
            get
            {
                if (selfGo != null)
                {
                    return selfGo;
                }

                selfGo = RectTransform.gameObject;
                return selfGo;
            }
        }

        #region 释放及处理数据变更

        public virtual void Release()
        {
        }

        #endregion

        #region 元数据构建

        public abstract void Metamorphose(LegoUIMeta uiMeta);

        public LegoMetamorphoseStage MetamorphoseStage { get; protected set; }
        public void Construct(ILegoUI locUI, object obj = null)
        {
            LocUI = locUI;
        }

        #endregion
    }
}