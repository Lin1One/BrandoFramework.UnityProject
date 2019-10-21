namespace GameWorld.Editor
{

    public class GroupByAlreadyAdded : IGroupByFilter
    {
        public string GetName()
        {
            return "AlreadyAdded";
        }

        public string GetDescription(GameObjectFilterInfo fi)
        {
            return "[alreadyAdded] : " + fi.alreadyInBakerList;
        }

        public int Compare(GameObjectFilterInfo a, GameObjectFilterInfo b)
        {
            int alreadyAddedCompare = 0;
            if (b.alreadyInBakerList == true && a.alreadyInBakerList == false)
            {
                alreadyAddedCompare = -1;
            }
            if (b.alreadyInBakerList == false && a.alreadyInBakerList == true)
            {
                alreadyAddedCompare = 1;
            }
            return alreadyAddedCompare;
        }
    }
}



