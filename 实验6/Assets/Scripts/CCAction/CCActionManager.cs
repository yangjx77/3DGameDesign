using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//该游戏魔鬼与牧师的动作管理的具体实现类
public class CCActionManager : SSActionManager, ISSActionCallback
{
    //是否正在进行运动
    private bool isMoving = false;
    //表示船移动动作类（只需要往左或者右移动，不需要组合）
    public CCMoveToAction moveBoatAction;
    //角色移动动作类(需要组合——先往右平移，然后往下面移动)
    public CCSequenceAction moveRoleAction;
    //控制游戏运行的主控制器
    public FirstController mainController;

    //重写基类的Start方法
    protected new void Start()
    {
        //获取主控制器的引用，并且将当前的控制器设置为主控制器
        mainController = (FirstController)SSDirector.GetInstance().CurrentSceneController;
        mainController.actionManager = this;
    }

    //获取该游戏是否正在进行运动的状态
    public bool IsMoving()
    {
        return isMoving;
    }

    //移动船
    //创建移动船的动作并且将其添加到船上
    public void MoveBoat(GameObject boat, Vector3 target, float speed)
    {
        if (isMoving)
            return;
        isMoving = true;
        moveBoatAction = CCMoveToAction.GetSSAction(target, speed);
        this.RunAction(boat, moveBoatAction, this);
    }

    //移动人
    //创建人移动的动作，并且通过组合多个动作来实现从起点到中间位置然后再到目标位置的移动
    public void MoveRole(GameObject role, Vector3 mid_destination, Vector3 destination, int speed)
    {
        if (isMoving)
            return;
        isMoving = true;
        moveRoleAction = CCSequenceAction.GetSSAction(0, 0, new List<SSAction> { CCMoveToAction.GetSSAction(mid_destination, speed), CCMoveToAction.GetSSAction(destination, speed) });
        this.RunAction(role, moveRoleAction, this);
    }

    //回调函数
    //当一个动作完成后，将会调用该方法
    //将isMoving设置为false，表示动作的运动已经完成
    public void SSActionEvent(SSAction source,
    SSActionEventType events = SSActionEventType.Competeted,
    int intParam = 0,
    string strParam = null,
    Object objectParam = null)
    {
        isMoving = false;
    }
}
