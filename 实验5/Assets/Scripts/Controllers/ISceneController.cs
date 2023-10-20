using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用于场景控制的接口
//而其中的LoadResources方法用于加载场景所需的资源
public interface ISceneController
{
    void LoadResources();
}
