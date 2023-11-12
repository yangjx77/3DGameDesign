using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

//主控制器
//连接用户与游戏，分别需要实现场景控制器的接口和用户操作的接口
public class RoundController : MonoBehaviour, ISceneController, IUserAction
{
    //表示当前游戏的回合数
    int round = 0;
    //表示游戏的最大回合数
    int max_round = 10;
    //表示每回合的计时器
    float timer = 0.5f;
    //游戏对象，表示飞碟游戏对象
    GameObject disk;
    //飞碟工厂类型，用于创建飞碟对象
    DiskFactory factory ;
    //实现了IActionManager接口的变量，用于处理飞碟对象的动作（运动学和刚体物理）
    public IActionManager actionManager;
    //用于管理得分
    public ScoreController scoreController;
    //用于管理用户界面
    public UserGUI userGUI;
    // Start is called before the first frame update
    void Start(){}

    // Update is called once per frame
    void Update()
    {
        //首先检查userGUI的模式是否为0，如果是就直接返回
        if (userGUI.mode == 0) return;
        //接着根据userGUI的isKinematic属性选择合适的actionManager
        //如果isKinematic是false，表明对象的动作为物理刚体运动，就需要使用PhysicActionManager
        if (userGUI.isKinematic == false) {
            actionManager = gameObject.GetComponent<PhysicActionManager>() as IActionManager;
        }
        //如果为True，就表明对象的动作为运动学动作，就需要使用CCActionManager
        else {
            actionManager = gameObject.GetComponent<CCActionManager>() as IActionManager;
        }
        //调用GetHit()方法处理玩家的点击操作
        GetHit();
        //调用gameOver()方法检查游戏是否结束
        gameOver();
        //如果当前回合数大于最大回合数，就直接返回，不再执行后续的操作
        if (round > max_round) {
            return;
        }
        //计时器递减
        timer -= Time.deltaTime;
        //判断计时器是否小于等于0并且actionManager的剩余动作是否为0
        if (timer <= 0 && actionManager.RemainActionCount() == 0) {
            //如果满足条件，就从工厂中得到10个飞碟，为其加上动作
            for (int i = 0; i < 10; ++i) {
                disk = factory.GetDisk(round);
                //使用Fly函数为飞碟添加动作
                actionManager.Fly(disk);
            }
            //回合数加1
            round += 1;
            //如果回合数小于等于最大回合数
            //将回合数更新到userGUI的round上
            if (round <= max_round) {
                userGUI.round = round;
            }
            //将timer重置到4.0f
            timer = 4.0f;
        }
    }

    //Awake函数，该方法在脚本被唤醒时执行一次
    void Awake() {
        //首先通过SSDirector.getInstance()获取SSDirector的实例
        SSDirector director = SSDirector.getInstance();
        //并将当前场景的控制器设置为RoundController
        director.currentSceneController = this;
        //调用LoadSource()方法加载资源
        director.currentSceneController.LoadSource();
        //通过gameObject.AddComponent<T>()方法向游戏对象添加多个组件
        //依次为UserGUI、PhysicActionManager、CCActionManager、ScoreController和DiskFactory
        gameObject.AddComponent<UserGUI>();
        gameObject.AddComponent<PhysicActionManager>();
        gameObject.AddComponent<CCActionManager>();
        gameObject.AddComponent<ScoreController>();
        gameObject.AddComponent<DiskFactory>();
        //通过Singleton<DiskFactory>.Instance 获取DiskFactory的单实例
        //将获取到的单实例赋值给factory
        factory = Singleton<DiskFactory>.Instance;
        //通过gameObject.GetComponent<UserGUI>()获取当前游戏对象上的UserGUI组件
        //将结果赋值给当前的userGUI
        userGUI = gameObject.GetComponent<UserGUI>();
    }

    public void LoadSource() {}

    //用于判断游戏是否结束
    public void gameOver() 
    {
        //判断当前回合数大于最大回合数并且actionManager的剩余动作数量为0
        //将userGUI的gameMessage设置为"Game Over!"，表示游戏结束
        if (round > max_round && actionManager.RemainActionCount() == 0)
            userGUI.gameMessage = "Game Over!";
    }

    //用于处理玩家的点击操作
    public void GetHit() {
        //首先通过Input.GetButtonDown("Fire1")来判断玩家是否按下鼠标左键
        if (Input.GetButtonDown("Fire1")) {
            //如果是，就创建一条射线，其原点为摄像机，方向为鼠标点击位置
			Camera ca = Camera.main;
			Ray ray = ca.ScreenPointToRay(Input.mousePosition);

            //通过Physics.Raycast(ray, out hit)返回射线与场景物体的碰撞结果
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
                //如果射线与物体发生碰撞，则调用scoreController.Record()方法记录得分
                //将碰撞到的物体设置为非活动状态，即隐藏该物体
                scoreController.Record(hit.transform.gameObject);
                hit.transform.gameObject.SetActive(false);
			}
		}
    }
}
