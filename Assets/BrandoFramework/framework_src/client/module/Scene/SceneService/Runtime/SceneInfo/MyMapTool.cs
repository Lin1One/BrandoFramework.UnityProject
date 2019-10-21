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

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameWorld
{
    public class MyMapTool
    {
        public static EMapLayer GetELaterByName(string objName, string rootName)
        {
            EMapLayer layer;

            if (objName == rootName)
                layer = EMapLayer.MapRoot;
            else
            {
                switch (objName)
                {
                    case "Terrain":
                        layer = EMapLayer.Terrain;
                        break;
                    case "Building":
                        layer = EMapLayer.Building;
                        break;
                    case "Other":
                        layer = EMapLayer.Other;
                        break;
                    case "Tree":
                        layer = EMapLayer.Tree;
                        break;
                    case "AdornEffect":
                        layer = EMapLayer.AdornEffect;
                        break;
                    case "Shadow":
                        layer = EMapLayer.Shadow;
                        break;
                    case "Animation":
                        layer = EMapLayer.Animation;
                        break;
                    default:
                        layer = EMapLayer.None;
                        break;
                }
            }
            return layer;
        }

        public static string GetNameByELater(EMapLayer eMapLayer, string rootName)
        {
            string name;

            switch (eMapLayer)
            {
                case EMapLayer.Terrain:
                    name = "Terrain";
                    break;
                case EMapLayer.Building:
                    name = "Building";
                    break;
                case EMapLayer.Other:
                    name = "Other";
                    break;
                case EMapLayer.Tree:
                    name = "Tree";
                    break;
                case EMapLayer.AdornEffect:
                    name = "AdornEffect";
                    break;
                case EMapLayer.Shadow:
                    name = "Shadow";
                    break;
                case EMapLayer.Animation:
                    name = "Animation";
                    break;
                case EMapLayer.MapRoot:
                    name = rootName;
                    break;
                case EMapLayer.None:
                    name = null;
                    break;
                default:
                    name = null;
                    break;
            }
            return name;
        }
    }

    //服务器需要的场景数据
    [Serializable]
    public class SceneVO
    {
        public int id;  //地图id

        public string name;

        public int width = 600;

        public int height = 600;

        public int tileWH = 1;      //格子宽度      （可以写死，代码中判断跨格都是以1为标准的）
            
        public int tiltTH = 1;      //格子高度      （可以写死，代码中判断跨格都是以1为标准的）

        public int[] block;        //false可走  true阻挡

        public int[] box;        //刷宝箱点

        public int backWH = 0;  //二层背景宽             （3D无用）

        public int backTH = 0;  //二层背景高     （3D无用）

        public int moveWH = 0;      //移动背景宽     （3D无用）

        public int moveTH = 0;  //移动背景高     （3D无用）

        //public int[] flag;     //0 无图片   1 有图片有透明区域   2 有图片不透明      （3D无用）

        //public int[] alpha;    //透明     （3D无用）

        //public int[] safe;     //false危险   true安全      (安全区)

        //public int[] path;     //false非路径  true路径      （跳跃点）

        //public MonsterUnit[] monsterUnits;


    }

    [Serializable]
    public class MonsterUnit
    {
        public int team;

        public int id;

        public string name;

        public int maxCount;

        public MyVector2[] createPoints;
    }


    [Serializable]
    public class MyVector3
    {
        public float x;
        public float y;
        public float z;
        
        public MyVector3()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public MyVector3(Vector3 vec)
        {
            x = vec.x;
            y = vec.y;
            z = vec.z;
        }

        public MyVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [Serializable]
    public class MyVector2
    {
        public float x;
        public float y;

        public MyVector2()
        {
            x = 0;
            y = 0;
        }

        public MyVector2(Vector2 vec)
        {
            x = vec.x;
            y = vec.y;
        }

        public MyVector2(float x,float y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }
    }

    [Serializable]
    public class MyVector4
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public MyVector4()
        {
            x = 0;
            y = 0;
            z = 0;
            w = 0;
        }

        public MyVector4(Vector4 vec)
        {
            x = vec.x;
            y = vec.y;
            z = vec.z;
            w = vec.w;
        }

        public MyVector4(float x, float y,float z,float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Vector4 ToVector4()
        {
            return new Vector4(x, y, z,w);
        }
    }

    /// <summary>
    /// 区域之间的跳跃点信息
    /// </summary>
    public class YuMapAreaInfo
    {
        public YuMapAreaInfo(YuMapJumpPoint[] jumpArr)
        {
            JumpPoints = jumpArr;
        }

        private YuMapJumpPoint[] JumpPoints;

        public int Count
        {
            get { return JumpPoints.Length; }
        }

        public YuMapJumpPoint this[int index]
        {
            get { return JumpPoints[index]; }
        }
    }

    /// <summary>
    /// 一个跳跃点信息
    /// </summary>
    public class YuMapJumpPoint
    {
        public int StartAreaId;
        public int EndAreaId;
        public Vector2 StartPosition;
        public Vector2 EndPosition;
    }
}