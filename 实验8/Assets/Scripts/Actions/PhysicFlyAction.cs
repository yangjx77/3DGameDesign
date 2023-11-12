using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//飞碟动作类（物理刚体运动）
//只需要水平方向上的初速度，在垂直方向上考虑重力
//飞碟从界面左右两侧飞入，离开界面时运动结束
public class PhysicFlyAction : SSAction
{
    //只需要定义一个变量用于1存储水平方向上的运动速度
    public float speedX;
    //用于创建一个PhysicFlyAction实例，并且设置其速度
    //返回创建的该实例
    public static PhysicFlyAction GetSSAction(float x) {
        PhysicFlyAction action = ScriptableObject.CreateInstance<PhysicFlyAction>();
        action.speedX = x;
        return action;
    }
    // Start is called before the first frame update
    public override void Start()
    {
        //将飞碟的刚体的isKinematic设置为了False，表示这是一个刚体的物理运动
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        //设置飞碟的初始速度
        gameObject.GetComponent<Rigidbody>().velocity = new Vector3(speedX * 10, 0, 0);
        //将飞碟的阻力设置为1，以模拟飞碟在空气中受到的空气阻力
        gameObject.GetComponent<Rigidbody>().drag = 1;
    }

    //与运动学的动作不同，这里不需要不断更行飞碟的位置
    //只需要给飞碟一个初速度以及重力，飞碟自己就会运动
    // Update is called once per frame
    public override void Update()
    {
        //检查飞碟是否处于非激活状态（被销毁）
        if (this.transform.gameObject.activeSelf == false) {
            //如果是，将destroy设置为True
            this.destroy = true;
            //调用回调接口的方法通知动作已经结束
            this.callback.SSActionEvent(this);
            //然后返回
            return;
        }
        
        //将飞碟的世界坐标转换为屏幕坐标
        Vector3 vec3 = Camera.main.WorldToScreenPoint (this.transform.position);
        //判断飞碟是否超出了屏幕的范围
        if (vec3.x < -100 || vec3.x > Camera.main.pixelWidth + 100 || vec3.y < -100 || vec3.y > Camera.main.pixelHeight + 100) {
            //如果超出，将destroy设置为True
            this.destroy = true;
            //调用回调接口的方法通知该动作已经结束
            this.callback.SSActionEvent(this);
            //然后返回
            return;
        }
    }
}

