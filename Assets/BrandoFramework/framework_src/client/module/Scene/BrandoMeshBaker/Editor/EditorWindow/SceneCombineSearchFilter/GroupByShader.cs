namespace GameWorld.Editor
{
    public class GroupByShader : IGroupByFilter
    {
        public string GetName()
        {
            return "Shader";
        }

        public string GetDescription(GameObjectFilterInfo fi)
        {
            return "[Shader] : " + fi.shaderName;
        }

        public int Compare(GameObjectFilterInfo a, GameObjectFilterInfo b)
        {
            return a.shaderName.CompareTo(b.shaderName);
        }
    }

}



