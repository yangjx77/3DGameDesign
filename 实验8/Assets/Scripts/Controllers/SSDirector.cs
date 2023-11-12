using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//导演类
public class SSDirector : System.Object
{
    //用于声明一个私有的静态字段_instance，用于存储SSDirector的唯一实例
    private static SSDirector _instance;
    //定义了一个公共的属性currentSceneController
    //用于获取或设置当前场景控制器，该属性的类型为ISceneController接口
    public ISceneController currentSceneController {get; set;}
    
    //用于获取SSDirector的实例
    public static SSDirector getInstance() {
        //首先检查_instance是否为空
        if (_instance == null) {
            //如果为空，就创建一个新的SSDirector实例
            //并且将该实例赋值给_instance
            _instance = new SSDirector();
        }
        //最后返回_instance
        return _instance;
    }
}
