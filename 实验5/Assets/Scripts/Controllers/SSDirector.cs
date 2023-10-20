using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//实现了单例模式，保证了在整个游戏的过程中只有一个实例存在
//方便访问和管理游戏中的全局对象，协调游戏场景和场景控制器
public class SSDirector : System.Object
{
    //静态变量，用于保存类的唯一实例
    static SSDirector _instance;
    //用于获取和设置当前场景的控制器对象
    public ISceneController CurrentSceneController {get; set;}
    //用于获取类的实例
    public static SSDirector GetInstance() {
        if (_instance == null) {
            _instance = new SSDirector();
        }
        return _instance;
    }
}
