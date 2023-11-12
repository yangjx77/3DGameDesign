using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//分数控制器
//用于记录游戏中的得分
public class ScoreController : MonoBehaviour
{
    //用于存储游戏得分
    int score;
    //用于引用当前场景的RoundController控制器
    public RoundController roundController;
    //用于引用当前场景的UserGUI组件
    public UserGUI userGUI;
    // Start is called before the first frame update
    void Start()
    {
        //首先通过SSDirector.getInstance().currentSceneController获取当前场景的控制器
        //并且将其转换为RoundController的类型
        //同时将结果赋值给roundController
        roundController = (RoundController)SSDirector.getInstance().currentSceneController;
        //将当前的ScoreController实例赋值给roundController的scoreController属性
        roundController.scoreController = this;
        //通过GetComponent<UserGUI>()方法获取当前游戏对象上的UserGUI组件，并将结果赋值给userGUI
        userGUI = this.gameObject.GetComponent<UserGUI>();
    }

    //用于记录得分
    public void Record(GameObject disk) {
        //通过disk.GetComponent<DiskAttributes>().score获取disk游戏对象上的DiskAttributes组件，并获取其score属性的值，
        //然后将其累加到score变量上
        score += disk.GetComponent<DiskAttributes>().score;
        //接着，将累加后的score赋值给userGUI的score属性
        userGUI.score = score;
    }
}
