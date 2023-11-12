using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用户界面
public class UserGUI : MonoBehaviour
{
    //用于记录用户处于哪一个界面
    public int mode;
    //用于记录用户的总得分
    public int score;
    //用于记录游戏处于的游戏轮数
    public int round;
    //用于记录需要返回给用户的游戏描述
    public string gameMessage;
    //用于记录当前飞碟的动作为运动学还是物理刚体
    public bool isKinematic;
    //表示用户行为动作接口
    private IUserAction action;
    //用于定义自定义字体格式
    public GUIStyle bigStyle, blackStyle, smallStyle;
    //用于存储像素字体
    public Font pixelFont;
    //表示主菜单每一个按键的宽度和高度
    private int menu_width = Screen.width / 5, menu_height = Screen.width / 10;
    // Start is called before the first frame update
    void Start()
    {
        //首先进行初始化，将动作初始化为运动学动作
        isKinematic = true;
        //将当前的用户界面初始化为0
        mode = 0;
        //将需要显示的信息初始化为空字符
        gameMessage = "";
        //获取SSDirector的实例，并将其当前场景控制器转换为IUserAction接口类型，然后赋值给action变量
        action = SSDirector.getInstance().currentSceneController as IUserAction;
        
        //大字体初始化
        //创建一个名为bigStyle的新GUIStyle对象
        bigStyle = new GUIStyle();
        //设置bigStyle的文本颜色为白色
        bigStyle.normal.textColor = Color.white;
        //设置bigStyle的背景为 null
        bigStyle.normal.background = null;
        //设置bigStyle的字体大小为 50
        bigStyle.fontSize = 50;
        //设置bigStyle的对齐方式为居中
        bigStyle.alignment=TextAnchor.MiddleCenter;

        //类似地，对blackStyle进行了相应的初始化
        //black
        blackStyle = new GUIStyle();
        blackStyle.normal.textColor = Color.black;
        blackStyle.normal.background = null;
        blackStyle.fontSize = 50;
        blackStyle.alignment=TextAnchor.MiddleCenter;

        //小字体初始化
        //类似地，对smallStyle进行了相应的初始化
        smallStyle = new GUIStyle();
        smallStyle.normal.textColor = Color.white;
        smallStyle.normal.background = null;
        smallStyle.fontSize = 20;
        smallStyle.alignment=TextAnchor.MiddleCenter;
    }

    // Update is called once per frame
    void Update(){}

    //是Unity的生命周期方法，在每个渲染帧之后被调用，用于绘制GUI元素
    void OnGUI() {
        GUI.skin.button.fontSize = 20;
        //根据mode的值进行分支调用
        switch(mode) {
            case 0:
                mainMenu();
                break;
            case 1:
                GameStart();
                break;
        }       
    }

    //主菜单界面
    void mainMenu() {
        //在指定位置绘制标签，显示文本为"Hit UFO"，使用预定义的bigStyle样式
        GUI.Label(new Rect(Screen.width / 2 - menu_width * 0.5f, Screen.height * 0.1f, menu_width, menu_height), "Hit UFO", bigStyle);
        //在指定位置绘制按钮，显示文本为"Start"，使用指定的位置和大小，返回一个布尔值表示按钮是否被点击
        bool button = GUI.Button(new Rect(Screen.width / 2 - menu_width * 0.5f, Screen.height * 3 / 7, menu_width, menu_height), "Start");
        //如果按钮被点击，就将mode设置为1，即会调用GameStart函数
        if (button) {
            mode = 1;
        }
    }
    //游戏开始的界面
    void GameStart() {
        //创建三个label
        //一个用于显示返回的游戏信息
        GUI.Label(new Rect(300, 60, 50, 200), gameMessage, bigStyle);
        //用于返回游戏的得分
        GUI.Label(new Rect(0,0,100,50), "Score: " + score, smallStyle);
        //用于返回游戏的轮数
        GUI.Label(new Rect(560,0,100,50), "Round: " + round, smallStyle);
        //在指定位置绘制按钮，显示文本为"Kinematic/Not Kinematic"，使用指定的位置和大小，返回一个布尔值表示该按钮是否被点击
        if (GUI.Button(new Rect(Screen.width / 2 - menu_width * 0.9f, 0, menu_width * 1.8f, menu_height), "Kinematic/Not Kinematic")) {
            //如果该按钮被点击
            //就将isKinematic设置为原来的负值
            isKinematic = !isKinematic;
        }
    }
}
