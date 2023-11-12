using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//场景控制器接口
public interface ISceneController 
{
    //用于加载资源的方法
    void LoadSource();
    //用于处理击中事件的方法
    void GetHit();
}
