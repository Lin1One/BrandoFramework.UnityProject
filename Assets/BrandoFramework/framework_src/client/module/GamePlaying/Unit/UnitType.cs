#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

namespace Client.GamePlaying.Unit
{
    public enum UnitType : byte
    {
        LeadPlayer, //主角
        FriendlyPlayer,     //其他玩家
        HostilePlayer, //敌对玩家
        Monster,    //怪物
        Boss,       //boss
        Npc,        //NPC
        Follower,   //随从
        Obstacle,    //障碍物
        DropItem, //掉落物品
        CollectItem,    //采集物
    }
}

