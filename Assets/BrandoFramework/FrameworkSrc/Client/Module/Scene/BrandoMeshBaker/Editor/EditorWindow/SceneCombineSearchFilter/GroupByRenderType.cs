namespace GameWorld.Editor
{

    public class GroupByRenderType : IGroupByFilter
    {
        public string GetName()
        {
            return "RenderType";
        }

        public string GetDescription(GameObjectFilterInfo fi)
        {
            return "[RenderType] : " + (fi.isMeshRenderer ? "MeshRenderer" : "SkinnedMeshRenderer");
        }

        public int Compare(GameObjectFilterInfo a, GameObjectFilterInfo b)
        {
            int renderTypeCompare = 0;
            if (b.isMeshRenderer == true && a.isMeshRenderer == false)
            {
                renderTypeCompare = -1;
            }
            if (b.isMeshRenderer == false && a.isMeshRenderer == true)
            {
                renderTypeCompare = 1;
            }
            return renderTypeCompare;
        }
    }
}



