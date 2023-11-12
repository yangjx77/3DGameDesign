using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//飞碟动作类（运动学）
//继承自动作基类
//飞碟从界面左右两侧飞入，离开界面时运动结束
//飞碟的运动有两个方向，主要为水平运动方向和垂直运动方向
public class CCFlyAction : SSAction
{
    //在水平方向上的运动速度
    public float speedX;
    //在垂直方向上的运动速度
    public float speedY;
    //用于创建并返回飞碟运动的实例
    //接收飞碟在水平和垂直方向上的速度并且设置速度
    //返回该飞碟运动的动作
    public static CCFlyAction GetSSAction(float x, float y) {
        CCFlyAction action = ScriptableObject.CreateInstance<CCFlyAction>();
        action.speedX = x;
        action.speedY = y;
        return action;
    }
    // Start is called before the first frame update
    public override void Start()
    {
        //将刚体的是否为运动学运动的属性变量设置为True
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    // Update is called once per frame
    public override void Update()
    {
        //首先检查飞碟游戏对象是否处于非激活状态（被销毁）
        if (this.transform.gameObject.activeSelf == false) {
            //将销毁destroy设置为True
            this.destroy = true;
            //通过回调函数接口通知事件发生
            this.callback.SSActionEvent(this);
            //随后返回
            return;
        }
        
        //将飞碟的世界坐标转换为屏幕坐标
        Vector3 vec3 = Camera.main.WorldToScreenPoint (this.transform.position);
        //如果飞碟已经超出了屏幕的范围，就将destroy设置为True，同时通过回调函数接口通知事件发生，同时返回
        if (vec3.x < -100 || vec3.x > Camera.main.pixelWidth + 100 || vec3.y < -100 || vec3.y > Camera.main.pixelHeight + 100) {
            this.destroy = true;
            this.callback.SSActionEvent(this);
            return;
        }
        //如果飞碟还在屏幕的范围内，就通过在水平和垂直的速度更新飞碟的速度状态
        transform.position += new Vector3(speedX, speedY, 0) * Time.deltaTime * 2;
    }
}
