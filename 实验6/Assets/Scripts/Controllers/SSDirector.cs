using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSDirector : System.Object
{
    //一个静态的SSDirector变量，用于保存单例实例
    static SSDirector _instance;
    //主要用于设置和存储当前场景的控制器
    public ISceneController CurrentSceneController {get; set;}
    //用于获取SSDirector的单例实例，
    //如果_instance为空，则创建一个新的SSDirector实例并赋值给_instance，然后返回_instance
    public static SSDirector GetInstance() {
        if (_instance == null) {
            _instance = new SSDirector();
        }
        return _instance;
    }
}
