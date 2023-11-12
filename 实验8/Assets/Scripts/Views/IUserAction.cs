using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用户接口
public interface IUserAction {
    //用于处理游戏结束的逻辑接口函数
    void gameOver();
    //用于处理玩家点击的逻辑接口函数
    void GetHit();
    
}
