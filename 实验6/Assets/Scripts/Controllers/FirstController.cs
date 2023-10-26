using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//这是本游戏的最核心的控制部分，协调所有游戏对象的实现与信息调试
public class FirstController : MonoBehaviour, ISceneController, IUserAction {
    public CCActionManager actionManager;
    //创建陆地的对象，为左岸以及右岸
    public LandControl leftLandController, rightLandController;
    //创建河流对象
    public River river;
    //创建一个船控制器，调用将角色移动到船上或者反过来的实现函数
    public BoatControl boatController;
    //创建一个角色控制器数组，每一个角色对象都需要一个独立的控制器
    public RoleControl[] roleControllers;
    //创建一个布尔变量，用于标记游戏是否在运行
    public bool isRunning;
    //创建一个变量，标记游戏的剩余时间
    public float time;

    //判定回调函数，用于处理游戏胜负判定的结果
    //根据传入的参数设置游戏时间，同时绑定isRunning
    public void JudgeCallback(bool isRuning, string message)
    {
        this.gameObject.GetComponent<UserGUI>().gameMessage = message;
        this.gameObject.GetComponent<UserGUI>().time = (int)time;
        this.isRunning = isRunning;
    }

    //用于加载资源、初始化游戏场景
    public void LoadResources() {
        //初始化每一个角色控制器
        roleControllers = new RoleControl[6];
        for (int i = 0; i < 6; ++i) {
            roleControllers[i] = new RoleControl();
            roleControllers[i].CreateRole(Position.role_land[i], i < 3 ? true : false, i);
        }

        //初始化陆地控制器
        //创建左岸与右岸以及获取名字
        leftLandController = new LandControl();
        leftLandController.CreateLand(Position.left_land);
        leftLandController.GetLand().land.name = "left_land";
        rightLandController = new LandControl();
        rightLandController.CreateLand(Position.right_land);
        rightLandController.GetLand().land.name = "right_land";

        //初始化地将人物添加并定位到左岸  
        foreach (RoleControl roleController in roleControllers)
        {
            roleController.GetRoleModel().role.transform.localPosition = leftLandController.AddRole2Land(roleController.GetRoleModel());
        }

        //初始化船控制器，创建一个船控制器即可
        boatController = new BoatControl();
        boatController.CreateBoat(Position.left_boat);

        //初始化一条河
        river = new River(Position.river);

        //更新游戏是否运行为True
        isRunning = true;

        //初始化游戏剩余时间为60秒
        time = 60;
    }

    // //实现移动船的函数
    public void MoveBoat() {
        //如果游戏没有运行或者运动管理器里面的获取对象是否移动为False时，就直接返回，函数结束
        if (isRunning == false || actionManager.IsMoving()) return;
        
        //根据船的当前位置确定目标位置，然后调用动作管理器的MoveBoat()方法移动船，并更新船的位置状态。
        Vector3 destination = boatController.GetBoatModel().isRight ? Position.left_boat : Position.right_boat;
        actionManager.MoveBoat(boatController.GetBoatModel().boat, destination, 5);
        
        boatController.GetBoatModel().isRight = !boatController.GetBoatModel().isRight;
    }

    // //实现角色的移动的函数
    public void MoveRole(Role roleModel) {
        //如果游戏没有运行或者动作管理器中的对象正在移动，则直接返回
        if (isRunning == false || actionManager.IsMoving()) return;
        Vector3 destination, mid_destination;
        //根据角色的位置和船的位置确定目标位置，然后调用动作管理器的MoveRole()方法移动角色，并更新角色的位置状态
        if (roleModel.inBoat)
        {
            
            if (boatController.GetBoatModel().isRight)
                destination = rightLandController.AddRole2Land(roleModel);
            else
                destination = leftLandController.AddRole2Land(roleModel);
            if (roleModel.role.transform.localPosition.y > destination.y)
                mid_destination = new Vector3(destination.x, roleModel.role.transform.localPosition.y, destination.z);
            else
                mid_destination = new Vector3(roleModel.role.transform.localPosition.x, destination.y, destination.z);
            actionManager.MoveRole(roleModel.role, mid_destination, destination, 5);
            roleModel.onRight = boatController.GetBoatModel().isRight;
            boatController.RemoveRole(roleModel);
        }
        else
        {
            if (boatController.GetBoatModel().isRight == roleModel.onRight)
            {
                if (roleModel.onRight)
                {
                    rightLandController.RemoveRole(roleModel);
                }
                else
                {
                    leftLandController.RemoveRole(roleModel);
                }
                destination = boatController.AddRole(roleModel);
                if (roleModel.role.transform.localPosition.y > destination.y)
                    mid_destination = new Vector3(destination.x, roleModel.role.transform.localPosition.y, destination.z);
                else
                    mid_destination = new Vector3(roleModel.role.transform.localPosition.x, destination.y, destination.z);
                actionManager.MoveRole(roleModel.role, mid_destination, destination, 5);
            }
        }
    }

    //在脚本实例中唤醒时调用的方法
    void Awake() {
        //设置单例模式的当前场景控制器为当前实例，并且加载资源并初始化游戏界面、动作管理器和判定控制器。
        SSDirector.GetInstance().CurrentSceneController = this;
        LoadResources();
        this.gameObject.AddComponent<UserGUI>();
        this.gameObject.AddComponent<CCActionManager>();
        this.gameObject.AddComponent<JudgeController>();
    }

    //更新函数
    //更新游戏的时间
    void Update() {
        if (isRunning)
        {
            time -= Time.deltaTime;
            this.gameObject.GetComponent<UserGUI>().time = (int)time;
        }
    }
}
