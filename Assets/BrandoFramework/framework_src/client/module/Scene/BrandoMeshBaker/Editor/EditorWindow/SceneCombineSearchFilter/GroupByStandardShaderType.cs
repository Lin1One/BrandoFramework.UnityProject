namespace GameWorld.Editor
{

    public class MB3_GroupByStandardShaderType : IGroupByFilter
    {
        public string GetName()
        {
            return "Standard Rendering Mode";
        }

        public string GetDescription(GameObjectFilterInfo fi)
        {
            return "renderingMode=" + fi.standardShaderBlendModesName;
        }

        public int Compare(GameObjectFilterInfo a, GameObjectFilterInfo b)
        {
            return a.standardShaderBlendModesName.CompareTo(b.standardShaderBlendModesName);
        }
    }

}



