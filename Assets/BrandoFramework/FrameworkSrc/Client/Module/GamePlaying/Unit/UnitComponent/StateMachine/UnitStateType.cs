#region Head

// Author:            Chengkefu
// CreateDate:        2018/10/10 9:49:00
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

namespace Client.GamePlaying.Unit
{
    public enum UnitActStateType
    {
        wait,  //等待状态切换中（用于缓冲）
        idle,   //待机
        move,   //移动
        skill,  //施放技能
        jump,   //跳跃
        die,    //死亡
        hit,    //受击
        interaction,    //互动
    }
}

