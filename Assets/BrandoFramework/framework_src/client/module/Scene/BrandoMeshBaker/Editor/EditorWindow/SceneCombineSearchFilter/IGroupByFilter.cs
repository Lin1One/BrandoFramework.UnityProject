namespace GameWorld.Editor
{
    public interface IGroupByFilter
    {

        /// <summary>
        /// 	this name appears in the dropdown list.
        /// </summary>
        /// <returns>The name.</returns>
        string GetName();

        /// <summary>
        /// 	returns a description of the game object for this filter
        ///     eg. renderType=MeshFilter
        /// </summary>
        /// <returns>The description.</returns>
        /// <param name="fi">Fi.</param>
        string GetDescription(GameObjectFilterInfo fi);

        /// <summary>
        ///      For sorting Similar to IComparer.Compare
        /// </summary>
        int Compare(GameObjectFilterInfo a, GameObjectFilterInfo b);
    }
}



