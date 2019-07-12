#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.LegoUI
{
    /// <summary>
    /// UI层即挂载点。
    /// </summary>
    public class YuLegoUILayer
    {
        /// <summary>
        /// 该ui层的ui根物体。
        /// </summary>
        private readonly RectTransform _uiRoot;

        /// <summary>
        /// 该ui层下每个视图的单位深度值。
        /// </summary>
        private readonly int _everyZ;

        private float _layerSpacing;

        private readonly LinkedList<IYuLegoView> _views
            = new LinkedList<IYuLegoView>();


        public float TopZ => _views.First?.Value.DepthZ ?? 0;
        public float BottomZ => _views.Last?.Value.DepthZ ?? 0;

        public YuLegoUILayer(RectTransform uiRoot, int everyZ, YuLegoUILayer nextLayer,float layerSpacing)
        {
            _uiRoot = uiRoot;
            _everyZ = everyZ;
            _layerSpacing = layerSpacing;
            NextLayer = nextLayer;
        }

        public YuLegoUILayer NextLayer { get; private set; }

        public void PushView(IYuLegoView view)
        {

            if (Math.Abs(BottomZ) < _layerSpacing)
            {
                var z = _views.Count * _everyZ;

                view.DepthZ = -z;
                _views.AddLast(view);
                return;
            }


            NextLayer.BackShift(_layerSpacing);
            _layerSpacing = _layerSpacing * 2;
            PushView(view);
        }


        private void BackShift(float layerSpacingOffset)
        {
            var transform = _uiRoot.transform;
            var localPosition = transform.localPosition;

            var rootX = localPosition.x; 
            var rootY = localPosition.y;
            var rootZ = localPosition.z;

            localPosition = new Vector3(rootX, rootY, rootZ - layerSpacingOffset);
            transform.localPosition = localPosition;

            NextLayer?.BackShift(layerSpacingOffset);
        }
    }
}