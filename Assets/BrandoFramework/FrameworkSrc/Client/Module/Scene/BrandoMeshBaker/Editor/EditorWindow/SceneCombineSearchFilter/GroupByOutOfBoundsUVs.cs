using System;

namespace GameWorld.Editor
{

    public class GroupByOutOfBoundsUVs : IGroupByFilter
    {
        public string GetName()
        {
            return "OutOfBoundsUVs";
        }

        public string GetDescription(GameObjectFilterInfo fi)
        {
            return "[OutOfBoundsUVs] : " + fi.outOfBoundsUVs;
        }

        public int Compare(GameObjectFilterInfo a, GameObjectFilterInfo b)
        {
            return Convert.ToInt32(b.outOfBoundsUVs) - Convert.ToInt32(a.outOfBoundsUVs);
        }
    }
}



