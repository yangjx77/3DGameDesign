using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//负责显示游戏界面和一些文本信息
public class UserGUI : MonoBehaviour
{
    //创建一个用户交互的对象，用于引入用户交互的接口
    IUserAction userAction;
    //创建一个变量用于存储需要显示的信息
    public string gameMessage ;
    //创建一个变量，用于计算游戏的剩余时间
    public int time;

    JudgeController judge;
    //unity内置的类
    GUIStyle style, bigstyle;
    // Start is called before the first frame update
    void Start()
    {
        //初始化一些样式，主要是字体显示的样式
        //初始化时间为60秒
        time = 60;
        //获取当前场景的用户操作接口，并且将其赋给userAction变量
        userAction = SSDirector.GetInstance().CurrentSceneController as IUserAction;

        style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 30;

        bigstyle = new GUIStyle();
        bigstyle.normal.textColor = Color.white;
        bigstyle.fontSize = 50;
    }

    // Update is called once per frame
    void OnGUI() {
        //绘制三个标签用户显示信息
        GUI.Label(new Rect(160, Screen.height * 0.1f, 50, 200), "Prieists and Devils", bigstyle);
        GUI.Label(new Rect(250, 100, 50, 200), gameMessage, style);
        GUI.Label(new Rect(0,0,100,50), "Time: " + time, style);
    }
}
