using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用于控制移动的行为
public class MoveControl 
{
    //创建一个游戏对象，表示需要移动的游戏对象
    GameObject moveObject;
    //用于获取移动状态
    public bool GetIsMoving() {
        //当需要移动的对象不为空，而且附加的Move组件的ismoving为True时，返回True
        return (moveObject != null && moveObject.GetComponent<Move>().isMoving == true);
    }

    //用于设置移动目标和相关参数，接收参数为目标位置以及一个需要移动的对象
    public void SetMove(Vector3 destination, GameObject moveObject) {
        //创建一个Move对象，用于对象的移动实现
        Move model;
        //将参数绑定
        this.moveObject = moveObject;
        //如果moveObject没有附加Move组件，那么为该游戏对象附加一个Move组件
        if (!moveObject.TryGetComponent<Move>(out model)) {
            moveObject.AddComponent<Move>();
        }

        //绑定Move组件中的移动目标位置
        this.moveObject.GetComponent<Move>().destination = destination;
        //如果需要移动对象的目标y位置大于目标的y位置，就将一个新的Vector3对象赋给移动对象的Move组件中的中间位置信息
        //其中x和z为目标的x和z，而y为对象本地的y
        if (this.moveObject.transform.localPosition.y > destination.y) {
            this.moveObject.GetComponent<Move>().mid_destination = new Vector3(destination.x, this.moveObject.transform.localPosition.y, destination.z);
        }
        else {
            //将一个新的Vector3对象赋给Move组件中的中间位置
            //其中x为移动对象的本地的x，而y和z为目标的y和z
            this.moveObject.GetComponent<Move>().mid_destination = new Vector3(this.moveObject.transform.localPosition.x, destination.y, destination.z);
        }
    }
}
