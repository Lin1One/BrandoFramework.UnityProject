using UnityEngine;

namespace GameWorld
{
    /// <summary>
    /// 网格合并入口，作为 Component 挂在于游戏物体上
    /// 实际实现合并操作为 MeshCombineHandler 对象
    /// </summary>
    public class MeshCombinerEntrance : MeshBakerCommon
    {
        [SerializeField]
        protected MeshCombineHandler _meshCombiner = new MeshCombineHandler();

        public override MeshCombineHandlerBase meshCombiner
        {
            get { return _meshCombiner; }
        }

        //public void BuildSceneMeshObject()
        //{
        //    _meshCombiner.BuildSceneMeshObject();
        //}

        //public virtual bool ShowHide(GameObject[] gos, GameObject[] deleteGOs)
        //{
        //    return _meshCombiner.ShowHideGameObjects(gos, deleteGOs);
        //}

        //public virtual void ApplyShowHide()
        //{
        //    _meshCombiner.ApplyShowHide();
        //}

        public override bool AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource)
        {
            _meshCombiner.name = name + "-mesh";
            return _meshCombiner.AddDeleteGameObjects(gos, deleteGOs, disableRendererInSource);
        }

        public override bool AddDeleteGameObjectsByID(GameObject[] gos, int[] deleteGOinstanceIDs, bool disableRendererInSource)
        {
            _meshCombiner.name = name + "-mesh";
            return _meshCombiner.AddDeleteGameObjectsByID(gos, deleteGOinstanceIDs, disableRendererInSource);
        }

        public void OnDestroy()
        {
            _meshCombiner.DisposeRuntimeCreated();
        }
    }
}