using Common.Utility;
using GameWorld;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Client.Scene.Editor
{
    [Serializable]
    public class SceneInfoInEditor
    {
        [Space(10)]
        [LabelText("场景 ID")]
        [LabelWidth(70)]
        public int mapId;

        [LabelText("场景名称")]
        [LabelWidth(70)]
        public string mapName;

        //分块加载数据
        [LabelText("x 轴格数")]
        [LabelWidth(70)]
        public int xCellCount;

        [LabelText("z 轴格数")]
        [LabelWidth(70)]
        public int zCellCount;

        [LabelText("x 轴分块大小")]
        [LabelWidth(70)]
        public float xCellSize;

        [LabelText("z 轴分块大小")]
        [LabelWidth(70)]
        public float zCellSize;

        [LabelText("场景原点")]
        [LabelWidth(60)]
        public Vector2 originPos;   //原点，（xz坐标最小点）

        [LabelText("场景格信息")]
        public SceneCellInfo[] cells;

        [LabelText("场景分层节点信息")]
        public SceneLayerInfo[] layers;

        [LabelText("场景物体信息")]
        public SceneItemInfo[] items;

        [BoxGroup("光照贴图", false)]
        [LabelText("光照贴图数据")]
        public SceneLightInfo lightmapInfo;

        [BoxGroup("寻路网格", false)]
        [LabelText("寻路网格数据")]
        public SceneNavMeshData navMeshData;


        [BoxGroup("高度信息", false)]
        [LabelText("是否包含高度信息")]
        public bool ConsiderHeightMap;

        [BoxGroup("高度信息")]
        [ShowIf("ConsiderHeightMap")]
        [LabelText("高度图原点X")]
        public float heightMapOriX;

        [BoxGroup("高度信息")]
        [ShowIf("ConsiderHeightMap")]
        [LabelText("高度图原点Z")]
        public float heightMapOriZ;

        [BoxGroup("高度信息")]
        [ShowIf("ConsiderHeightMap")]
        [LabelText("高度图单位像素对应X尺寸")]
        public float heightMapSizeX;

        [BoxGroup("高度信息")]
        [ShowIf("ConsiderHeightMap")]
        [LabelText("高度图单位像素对应Z尺寸")]
        public float heightMapSizeZ;

        [BoxGroup("高度信息")]
        [ShowIf("ConsiderHeightMap")]
        [LabelText("高度图对应最低高度")]
        public float heightMapMinY;

        [BoxGroup("高度信息")]
        [ShowIf("ConsiderHeightMap")]
        [LabelText("高度图的高度范围")]
        public float heightMapSizeY;

        [BoxGroup("实时光照", false)]
        [LabelText("场景是否有实时光")]
        public bool hasLight;

        [BoxGroup("实时光照")]
        [ShowIf("hasLight")]
        [LabelText("光源位置")]
        public Vector3 lightPos;

        [BoxGroup("实时光照")]
        [ShowIf("hasLight")]
        [LabelText("光源方向")]
        public Vector3 lightDir;

        [BoxGroup("实时光照")]
        [ShowIf("hasLight")]
        [LabelText("光源颜色")]
        public Color lightColor;

        [BoxGroup("实时光照")]
        [ShowIf("hasLight")]
        [LabelText("光源强度")]
        public float lightIntensity;

        [BoxGroup("雾效果", false)]
        [LabelText("场景是否有雾")]
        public bool hasFog;

        [BoxGroup("雾效果")]
        [ShowIf("hasFog")]
        [LabelText("雾颜色")]
        public Color fogColor;

        [BoxGroup("雾效果")]
        [ShowIf("hasFog")]
        [LabelText("模式")]
        public int fogMode;

        [BoxGroup("雾效果")]
        [ShowIf("hasFog")]
        [LabelText("开始距离")]
        public float fogStart;

        [BoxGroup("雾效果")]
        [ShowIf("hasFog")]
        [LabelText("结束距离")]
        public float fogEnd;

        [BoxGroup("雾效果")]
        [ShowIf("hasFog")]
        [LabelText("密度")]
        public float fogDensity;

        [LabelText("天空盒材质")]
        [LabelWidth(70)]
        public Material skyboxMaterial;

        //与服务器同步的信息
        //public MyVector2 oriPosVO;
        //public int xCellCountVO;
        //public int zCellCountVO;

        public int CellCount
        {
            get { return xCellCount * zCellCount; }
        }

        public float xMin
        {
            get { return originPos.x; }
        }

        public float zMin
        {
            get { return originPos.y; }
        }

        public float xLenth
        {
            get { return xCellCount * xCellSize; }
        }

        public float zLenth
        {
            get { return zCellCount * zCellSize; }
        }
    }

}