using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//这是本游戏的最核心的控制部分，协调所有游戏对象的实现与信息调试
public class FirstController : MonoBehaviour, ISceneController, IUserAction {
    //创建陆地的对象，为左岸以及右岸
    LandControl leftLandController, rightLandController;
    //创建河流对象
    River river;
    //创建一个船控制器，调用将角色移动到船上或者反过来的实现函数
    BoatControl boatController;
    //创建一个角色控制器数组，每一个角色对象都需要一个独立的控制器
    RoleControl[] roleControllers;
    //创建一个控制移动的控制器对象
    MoveControl moveController;
    //创建一个布尔变量，用于标记游戏是否在运行
    bool isRunning;
    //创建一个变量，标记游戏的剩余时间
    float time;

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

        //初始化移动控制器
        moveController = new MoveControl();

        //更新游戏是否运行为True
        isRunning = true;
        //初始化游戏剩余时间为60秒
        time = 60;
    }

    //实现移动船的函数
    public void MoveBoat() {
        //如果游戏没有运行或者移动控制器里面的获取对象是否移动为False时，就直接返回，函数结束
        if (isRunning == false || moveController.GetIsMoving()) return;
        //检查船的当前位置，然后设置船需要移动的目标位置（左岸或者右岸）
        if (boatController.GetBoatModel().isRight) {
            moveController.SetMove(Position.left_boat, boatController.GetBoatModel().boat);
        }
        else {
            moveController.SetMove(Position.right_boat, boatController.GetBoatModel().boat);
        }
        //更新参数
        boatController.GetBoatModel().isRight = !boatController.GetBoatModel().isRight;
    }

    //实现角色的移动的函数
    public void MoveRole(Role roleModel) {
        //如果游戏没有运行或者移动控制器里面的获取对象是否移动为False时，就直接返回，函数结束
        if (isRunning == false || moveController.GetIsMoving()) return;
        //如果角色在船上，那么根据船的位置设定移动的方向
        if (roleModel.inBoat) {
            if (boatController.GetBoatModel().isRight) {
                moveController.SetMove(rightLandController.AddRole2Land(roleModel), roleModel.role);
            }
            else {
                moveController.SetMove(leftLandController.AddRole2Land(roleModel), roleModel.role);
            }
            //检查角色的位置与岸的位置是否匹配，如果匹配，那么移动角色到岸上
            roleModel.onRight = boatController.GetBoatModel().isRight;
            boatController.RemoveRole(roleModel);
        }
        //如果角色在岸上，检查角色所在岸和船所在岸是否匹配，如果匹配，就将角色移动到船上
        else {
            if (boatController.GetBoatModel().isRight == roleModel.onRight) {
                if (roleModel.onRight) {
                    rightLandController.RemoveRole(roleModel);
                }
                else {
                    leftLandController.RemoveRole(roleModel);
                }
                moveController.SetMove(boatController.AddRole(roleModel), roleModel.role);
            }
        }
    }

    //检查游戏的状态与判断游戏是否获胜或者失败
    public void Check() {
        //如果游戏没有运行，那么直接返回，函数结束
        if (isRunning == false) return;
        //通过组件获取userGui的组件，并且将游戏提示清空
        this.gameObject.GetComponent<UserGUI>().gameMessage = "";
        //如果右边岸上的牧师等于三个，那么游戏结束，并且将游戏提示设为“You Win！”
        //同时更新游戏是否运行的参数
        if (rightLandController.GetLand().priestCount == 3) {
            this.gameObject.GetComponent<UserGUI>().gameMessage = "You Win!";
            isRunning = false;
        }
        //否则，如果左岸上的牧师数量不为0且小于魔鬼数量
        //或者右岸上的牧师数量不为0且小于魔鬼数量
        //或者游戏时间结束了
        //判断游戏结束，同时更新游戏是否运行的参数
        else {
            int leftPriestCount, rightPriestCount, leftDevilCount, rightDevilCount;
            leftPriestCount = leftLandController.GetLand().priestCount + (boatController.GetBoatModel().isRight ? 0 : boatController.GetBoatModel().priestCount);
            rightPriestCount = rightLandController.GetLand().priestCount + (boatController.GetBoatModel().isRight ? boatController.GetBoatModel().priestCount : 0);
            leftDevilCount = leftLandController.GetLand().devilCount + (boatController.GetBoatModel().isRight ? 0 : boatController.GetBoatModel().devilCount);
            rightDevilCount = rightLandController.GetLand().devilCount + (boatController.GetBoatModel().isRight ? boatController.GetBoatModel().devilCount : 0);
            if (leftPriestCount != 0 && leftPriestCount < leftDevilCount || rightPriestCount != 0 && rightPriestCount < rightDevilCount) {
                this.gameObject.GetComponent<UserGUI>().gameMessage = "Game Over!";
                isRunning = false;
            }
        }
    }

    //用于初始化场景和游戏对象
    //SSDirector是一个单例模式的类，通过GetInstance()方法获取实例
    //然后将当前场景控制器设置为当前对象，以便其他部分可以通过SSDirector来访问和管理场景控制器。
    void Awake() {
        SSDirector.GetInstance().CurrentSceneController = this;
        LoadResources();
        //向当前对象（即当前脚本所在的对象）添加了一个 UserGUI 组件。
        this.gameObject.AddComponent<UserGUI>();
    }

    //更新函数
    void Update() {
        //如果游戏正在运行，不断更新游戏结束的时间
        if (isRunning) {
            time -= Time.deltaTime;
            //将游戏剩余时间赋给组件的time属性，用于在界面上显示游戏剩余时间
            this.gameObject.GetComponent<UserGUI>().time = (int)time;
            //如果时间小于等于0，那么判断游戏结束，同时更新参数
            if (time <= 0) {
                this.gameObject.GetComponent<UserGUI>().time = 0;
                this.gameObject.GetComponent<UserGUI>().gameMessage = "Game Over!";
                isRunning = false;
            }
        }
    }
}
