using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//在本游戏中（魔鬼与牧师），由于只涉及到平移操作，而没有旋转、缩放操作
//所以我们只需要实现一个平移类即可
//主要负责运动
public class CCMoveToAction : SSAction
{
	//表示动作的目标位置
	public Vector3 target;
	//表示动作移动的速度
	public float speed;

	//构造函数
	private CCMoveToAction(){}

	//创建以及返回一个CCMoveToAction对象
	public static CCMoveToAction GetSSAction(Vector3 target, float speed){
		CCMoveToAction action = ScriptableObject.CreateInstance<CCMoveToAction> ();
		action.target = target;
		action.speed = speed;
		return action;
	}

	public override void Update ()
	{
		//在每一帧更新时，将游戏对象的位置逐渐移动到目标位置target，
		//并根据移动是否完成来设置destory属性和调用回调接口的SSActionEvent()方法
		if (this.gameobject == null||this.transform.localPosition == target) {
			//waiting for destroy
			this.destory = true;  
			this.callback.SSActionEvent (this);
			return;
		}
		this.transform.localPosition = Vector3.MoveTowards (this.transform.localPosition, target, speed * Time.deltaTime);
	}

	//重写了Start函数，但是没有具体实现
	public override void Start () {

	}
}

