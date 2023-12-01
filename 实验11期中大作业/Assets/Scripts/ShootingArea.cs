using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingArea : MonoBehaviour
{
    //将该区域的可用箭数初始化为5
    public int arrowCount = 10;
    //设置一个变量记录该区域是否有箭
    public bool isArrow;
    //记录玩家是否在区域内
    private bool isPlayer;

    private void OnTriggerStay(Collider other)
    {
        //如果玩家已经在区域内
        if (isPlayer) return;
        //如果该区域的触发器与标签为player的玩家发生碰撞
        if (other.gameObject.tag == "Player")
        {
            //更新相关的变量
            isPlayer = true;
            isArrow = true;
            //获取玩家物体上的脚本，并且将射击区域设置为当前的脚本
            other.gameObject.transform.GetComponent<Bow>().shootingArea = this;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        //如果触发器与标签为player的玩家离开碰撞
        if (other.gameObject.tag == "Player")
        {
            //更新相关变量
            isPlayer = false;
            if (other.gameObject.transform.GetComponent<Bow>().shootingArea != null)
            {
                //将射击区域内的箭矢数量赋值给玩家物体上的Bow脚本的射击区域的箭矢数量
                arrowCount = other.gameObject.transform.GetComponent<Bow>().shootingArea.arrowCount;
            }
            isArrow = false;
            //将玩家物体上的Bow脚本的射击区域设置为null
            other.gameObject.transform.GetComponent<Bow>().shootingArea = null;
        }
    }
}
