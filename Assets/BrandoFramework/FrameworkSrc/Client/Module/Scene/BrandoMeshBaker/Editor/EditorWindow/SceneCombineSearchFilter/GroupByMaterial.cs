namespace GameWorld.Editor
{
    public class GroupByMaterial : IGroupByFilter
    {
        public string GetName()
        {
            return "Material";
        }

        public string GetDescription(GameObjectFilterInfo fi)
        {
            return "[material] : " + fi.materialName;
        }

        public int Compare(GameObjectFilterInfo a, GameObjectFilterInfo b)
        {
            return a.materialName.CompareTo(b.materialName);
        }
    }
}



