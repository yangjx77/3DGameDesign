using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgeController : MonoBehaviour
{
    //控制游戏的主控制器
    public FirstController mainController;
    //游戏的左岸
    public Land leftLandModel;
    //游戏的右岸
    public Land rightLandModel;
    //游戏的船模型
    public Boat boatModel;

    // Start is called before the first frame update
    //在游戏开始时调用，主要用于获取主要控制器、左岸、右岸、船的引用
    void Start()
    {
        mainController = (FirstController)SSDirector.GetInstance().CurrentSceneController;
        this.leftLandModel = mainController.leftLandController.GetLand();
        this.rightLandModel = mainController.rightLandController.GetLand();
        this.boatModel = mainController.boatController.GetBoatModel();
    }

    // Update is called once per frame
    void Update()
    {
        //首先检查游戏是否在运行，如果不是就返回
        if (!mainController.isRunning)
            return;
        //检查游戏时间是否已经变为0，如果是就返回，并且调用回调函数同时传递Game Over以及false参数
        //同时将游戏是否运行设置为false
        if (mainController.time <= 0)
        {
            mainController.JudgeCallback(false, "Game Over!");
            mainController.isRunning=false;
            return;
        }
        this.gameObject.GetComponent<UserGUI>().gameMessage = "";

        //判断游戏是否已经胜利
        //如果右岸上的牧师数量已经达到了三个，那么游戏胜利，同时利用回调函数传递You Win以及false参数
        //同时将游戏是否运行设置为false
        if (rightLandModel.priestCount == 3)
        {
            mainController.JudgeCallback(false, "You Win!");
            mainController.isRunning=false;
            return;
        }
        else
        {
            //如果左岸上的牧师数量不为0，而且左岸上的牧师数量小于魔鬼数量，那么判断游戏失败
            //同时返回Game Over以及false参数
            //同时将游戏是否运行设置为false
            int leftPriestNum, leftDevilNum, rightPriestNum, rightDevilNum;
            leftPriestNum = leftLandModel.priestCount + (boatModel.isRight ? 0 : boatModel.priestCount);
            leftDevilNum = leftLandModel.devilCount + (boatModel.isRight ? 0 : boatModel.devilCount);
            if (leftPriestNum != 0 && leftPriestNum < leftDevilNum)
            {
                mainController.JudgeCallback(false, "Game Over!");
                mainController.isRunning=false;
                return;
            }
            rightPriestNum = rightLandModel.priestCount + (boatModel.isRight ? boatModel.priestCount : 0);
            rightDevilNum = rightLandModel.devilCount + (boatModel.isRight ? boatModel.devilCount : 0);
            //如果右岸上的牧师数量不为0，而且右岸上的牧师数量小于魔鬼数量，那么判断游戏失败
            //同时返回Game Over以及false参数
            //同时将游戏是否运行设置为false
            if (rightPriestNum != 0 && rightPriestNum < rightDevilNum)
            {
                mainController.JudgeCallback(false, "Game Over!");
                mainController.isRunning=false;
                return;
            }
        }
    }
}

