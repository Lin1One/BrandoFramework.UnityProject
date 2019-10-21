#region Head

// Author:            LinYuzhou
// CreateDate:        2019/9/18 01:42:04
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using GameWorld;
using System;

namespace Client.Scene
{
    public class MyPathManager
    {
        public static string SavePath = "Assets/Resources/Map";
    }

    [Serializable]
    public class SceneInfo
    {

        public int mapId;
        public string mapName;

        //天空盒
        public string skyboxMaterial;
        //雾
        public bool hasFog;
        public MyVector4 fogColor;
        public int fogMode;
        public float fogStart;
        public float fogEnd;
        public float fogDensity;

        //主方向光源数据
        public bool hasLight;
        public MyVector3 lightPos;
        public MyVector3 lightDir;
        public MyVector4 lightColor;
        public float lightIntensity;


        //分块加载数据
        public MyVector2 originPos;   //原点，（xz坐标最小点）
        public float xCellSize;     //单个格子x长度
        public float zCellSize;     //当个格子z长度
        public int xCellCount;
        public int zCellCount;

        //高度图数据
        public float heightMapOriX;     //高度图原点X
        public float heightMapOriZ;     //高度图原点Z
        public float heightMapSizeX;   //高度图单位像素对应X尺寸
        public float heightMapSizeZ;   //高度图单位像素对应Z尺寸
        public float heightMapMinY;     //高度图对应最低高度
        public float heightMapSizeY;    //高度图的高度范围

        //与服务器同步的信息
        public MyVector2 oriPosVO;
        public int xCellCountVO;
        public int zCellCountVO;

        public SceneCellInfo[] cells;
        public SceneLayerInfo[] layers;
        public SceneItemInfo[] items;

        public SceneLightInfo lightmapInfo;
        public SceneNavMeshData navMeshData;

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