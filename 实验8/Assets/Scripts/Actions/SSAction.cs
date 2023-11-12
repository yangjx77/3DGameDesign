using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//动作基类
//使用ScriptableObject文件定制属性配置
//创建可在编辑器和运行中使用的自定义脚本化文件
public class SSAction : ScriptableObject
{
    //表示是否启用该动作
    public bool enable = true;
    //表示是否销毁该动作
    public bool destroy = false;
    //用于获取和设置与该动作关联的游戏对象
    public GameObject gameObject { get; set;}
    //用于获取和设置与该动作关联的游戏对象的变换组件
    public Transform transform {get; set;}
    //用于获取和设置与该动作关联的回调函数
    public IActionCallback callback {get; set;}

    //无参构造函数，用于创建实例
    protected SSAction() {}
    // Start is called before the first frame update
    //在子类中要求实现，如果没有实现，就抛出异常
    public virtual void Start()
    {
        throw new System.NotImplementedException();
    }

    // Update is called once per frame
    //在子类中要求实现，如果没有实现，就抛出异常
    public virtual void Update()
    {
        throw new System.NotImplementedException();
    }
}
