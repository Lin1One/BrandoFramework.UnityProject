using Common.DataStruct;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Client.Scene.Editor
{
    [Serializable]
    public class SceneCellEditor
    {
        [LabelText("场景格列表")]
        public List<SceneCellInfoInEditor> cellInfos;

        [PropertySpace(10, 10)]
        [InlineButton("GetCurrentCellGameObjects","获取该 ID 格信息")]
        [LabelText("当前格")]
        [LabelWidth(70)]
        public int CurrentCellID;

        [LabelText("当前场景格所有物体")]
        public List<GameObject> CurrentCellItems;

        [PropertySpace(0,20)]
        [LabelText("当前场景格物体列表（不含跨格物体）")]
        public List<GameObject> OnlyInCurrentCellItems;

        [BoxGroup("场景合并")]
        [LabelText("物体合并分组方式")]
        [LabelWidth(100)]
        [InlineButton("GroupingGameObjects","游戏物体分组")]
        public CombineGroupType CombineInGroupBy;

        [BoxGroup("场景合并")]
        [FolderPath]
        [LabelText("场景格物体合并资源存放路径")]
        public string combineAssetFolder;

        [BoxGroup("场景合并")]
        [LabelText("通用合并设置")]
        public GameObjectCombineSetting combineSetting;

        [BoxGroup("场景合并")]
        [LabelText("当前场景格物体合并分组")]
        public List<SceneCellItemCombineGroup> cellItemGroups;


        [HorizontalGroup("合并操作")]
        [Button("合并材质")]
        public void CombineMaterial()
        {
            for(var i = 0;i<cellItemGroups.Count;i++)
            {
                var interalCombineSetting = cellItemGroups[i].groupCombineSetting;
                var combineSetting = interalCombineSetting.maxAtlasWidth != 0 ? interalCombineSetting : this.combineSetting;

                var combineData = new TextureCombinePipelineData();
                combineData.CombinedMaterialInfoPath = combineAssetFolder + "/Cell" + CurrentCellID + "_Group" + i + ".asset";
                combineData.DoMultiMaterial = combineSetting.DoMultiMaterial;

                TextureCombiner.CombineMaterial(cellItemGroups[i].groupGameObjects, combineSetting, combineData);

                //创建 TextureCombineResult
                //Texuture
                //Material
            }

        }

        [HorizontalGroup("合并操作")]
        [Button("合并网格并保存")]
        public void CombineMesh()
        {

        }

        public List<string> CellGameObjectRendererInfos;


        private void GetCurrentCellGameObjects()
        {
            ResetCurrentCellInfo();

            var currentCellInfo = cellInfos.Find(c => c.cellId == CurrentCellID);
            if (currentCellInfo != null)
            {
                var allGameObjects = SceneEditorDati.GetActualInstance().CurrentSceneInfo.items;
                var allGameObjectsList = allGameObjects.ToList();
                foreach(var gameObjectId in currentCellInfo.itemsNum)
                {
                    var gameObjectInfo = allGameObjectsList.Find(g => g.objNum == gameObjectId);
                    if(gameObjectInfo != null)
                    {
                        CurrentCellItems.Add(gameObjectInfo.obj);
                    }
                }
                foreach (var gameObjectId in currentCellInfo.OnlyInThisCellItemIds)
                {
                    var gameObjectInfo = allGameObjectsList.Find(g => g.objNum == gameObjectId);
                    if (gameObjectInfo != null)
                    {
                        OnlyInCurrentCellItems.Add(gameObjectInfo.obj);
                    }
                }
            }
        }   

        private void GroupingGameObjects()
        {
            cellItemGroups = SceneCellItemCombineGroup.Grouping(CombineInGroupBy, CurrentCellItems);
        }

        private void ResetCurrentCellInfo()
        {
            CurrentCellItems.Clear();
            OnlyInCurrentCellItems.Clear();
        }
    }

    [Serializable]
    public class SceneCellItemCombineGroup
    {
        public string groupInfo;
        public UnityEngine.Object groupByObj;
        public GameObjectCombineSetting groupCombineSetting;
        public List<GameObject> groupGameObjects = new List<GameObject>();

        public static List<SceneCellItemCombineGroup> Grouping(CombineGroupType groupBy,List<GameObject> gameObjects)
        {
            Dictionary<UnityEngine.Object, SceneCellItemCombineGroup> groupDic = new Dictionary<UnityEngine.Object, SceneCellItemCombineGroup>();

            foreach (var gameObject in gameObjects)
            {
                var renderer = gameObject.GetComponent<Renderer>();
                if(renderer == null)
                {
                    continue;
                }
                var shader = renderer.sharedMaterial.shader;
                if(!groupDic.ContainsKey(shader))
                {
                    var newGroup = new SceneCellItemCombineGroup();
                    groupDic.Add(shader, newGroup);
                    newGroup.groupByObj = shader;
                    newGroup.groupInfo  = shader.ToString();
                }
                groupDic[shader].groupGameObjects.Add(gameObject);
            }

            var groups = groupDic.ToValuesList();


            return groups;
        }
    }

    public enum CombineGroupType
    {
        ByShader,
    }
        

}