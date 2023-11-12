using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//提供一个最简单的接口给主控制器调用
public interface IActionManager
{
    //控制飞碟的飞行动作
    void Fly(GameObject disk);
    //用于返回当前回合飞碟的剩余数
    int RemainActionCount() ;
}
