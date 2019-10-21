using GameWorld;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
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
        public SceneCellInfoInEditor[] cells;

        [LabelText("场景分层节点信息")]
        public SceneLayerInfo[] layers;

        [LabelText("场景物体信息")]
        public SceneItemInfoInEditor[] items;

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

    //物体info
    [Serializable]
    public class SceneItemInfoInEditor
    {
        [LabelText("物体Id")]
        [LabelWidth(70)]
        public int objNum;

        [LabelWidth(70)]
        public GameObject obj;

        [LabelText("物体名称")]
        [LabelWidth(70)]
        public string objName;

        [LabelText("预制体名称")]
        [LabelWidth(70)]
        public string prefabName;

        [LabelText("资源路径")]
        [LabelWidth(70)]
        public string prefabAssetPath;

        [NonSerialized]
        public int useCount = 0;
        [LabelWidth(70)]
        public Vector3 pos = new Vector3();

        [LabelWidth(70)]
        public Vector3 rot = new Vector3();

        [LabelWidth(70)]
        public Vector3 scale = new Vector3();

        [LabelText("物体场景类型")]
        [LabelWidth(70)]
        public EMapLayer parentLayer;

        [LabelText("光照贴图ID")]
        public int[] lightmapIndexes;

        [LabelText("光照贴图偏移")]
        public MyVector4[] lightmapScaleOffsets;

        [LabelText("实时光照贴图ID")]
        public int[] realtimeLightmapIndexes;

        [LabelText("实时光照贴图偏移")]
        public MyVector4[] realtimeLightmapScaleOffsets;
    }

    //格子info
    [Serializable]
    public class SceneCellInfoInEditor
    {
        [LabelText("场景格ID")]
        [LabelWidth(70)]
        [InlineButton("SetCellItemsInScene","选择该格游戏物体")]
        public int cellId;

        [LabelText("场景物体ID列表")]
        public List<int> itemsNum;

        private void SetCellItemsInScene()
        {
            SceneEditorDati.GetActualInstance().SetSceneGameObjectInCell(cellId);
        }
    }

}