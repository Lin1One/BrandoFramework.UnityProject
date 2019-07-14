#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

namespace Client.GamePlaying.Unit
{
    public enum UnitType : byte
    {
        MainUnit, //����
        FriendlyPlayer,     //�������
        HostilePlayer, //�ж����
        Monster,    //����
        Boss,       //boss
        Npc,        //NPC
        Follower,   //���
        Obstacle,    //�ϰ���
        DropItem, //������Ʒ
        CollectItem,    //�ɼ���
    }
}

