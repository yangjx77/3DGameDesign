using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//（运动学）动作管理类
public class CCActionManager : SSActionManager, IActionCallback, IActionManager
{
    //定义一个控制器变量，用于获取当前场景控制器
    public RoundController sceneController;
    //用于存储飞碟的飞行动作（运动学）
    public CCFlyAction action;
    //用于获取飞碟的工厂实例
    public DiskFactory factory;
    
    // Start is called before the first frame update
    protected new void Start()
    {
        //获取当前场景控制器，并且调用导演类中的函数来确保场景的单实例
        sceneController = (RoundController)SSDirector.getInstance().currentSceneController;
        //将当前的CCActionManager设置为场景控制器的动作管理器
        sceneController.actionManager = this as IActionManager;
        //获取飞碟的工厂实例，确保工厂的单实例
        factory = Singleton<DiskFactory>.Instance;
    }

    //Update is called once per frame
    //protected new void Update(){}

    //重写回调函数
    public void SSActionEvent(SSAction source,
        SSActionEventType events = SSActionEventType.Completed,
        int intParam = 0,
        string strParam = null,
        Object objectParam = null) {
            //通过访问source的transform属性获取飞碟的游戏对象，并调用飞碟工厂的FreeDisk方法将飞碟回收
            factory.FreeDisk(source.transform.gameObject);
    }

    public override void MoveDisk(GameObject disk) {
        //根据飞碟的速度创建一个CCFlyAction动作实例
        action = CCFlyAction.GetSSAction(disk.GetComponent<DiskAttributes>().speedX, disk.GetComponent<DiskAttributes>().speedY);
        //将该动作实例作为参数传递给基类的RunAction方法，
        //同时传递当前的CCActionManager实例作为动作的回调接口
        RunAction(disk, action, this);
    }

    //这是IActionManager中的Fly函数，通过调用MoveDisk来控制飞碟的飞行
    public void Fly(GameObject disk) {
        MoveDisk(disk);
    }

    //返回当前正在（尚未）执行的飞碟的数量
    public int RemainActionCount() {
        return actions.Count;
    }
}
