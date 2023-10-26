using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//组合动作类，将事物的动作划分为多个小的动作
public class CCSequenceAction : SSAction, ISSActionCallback
{
	//一个List类型的数据，存储一系列SSAction对象，表示需要执行的动作序列
	public List<SSAction> sequence;
	//表示动作序列需要重复的次数，默认为-1，表示无限重复
	public int repeat = -1;
	//表示当前执行的动作在序列中的索引
	public int start = 0;

	//用于创建CCSequenceAction对象，
	//接受重复次数repeat、起始索引start和动作序列sequence作为参数，
	//返回一个CCSequenceAction对象
	public static CCSequenceAction GetSSAction(int repeat, int start , List<SSAction> sequence){
		CCSequenceAction action = ScriptableObject.CreateInstance<CCSequenceAction> ();
		action.repeat = repeat;
		action.sequence= sequence;
		action.start = start;
		return action;
	}
	
	// Update is called once per frame
	//重写当前的Update函数
	public override void Update ()
	{
		//如果sequence中的动作为空，那么直接返回
		if (sequence.Count == 0) return;  
		//如果当前还有未执行的动作，就再调用当前动作的Update方法
		if (start < sequence.Count) {
			sequence [start].Update ();
		}
	}

	//实现了ISSActionCallback接口的方法，
	//当一个动作完成时，会调用该方法，
	//将当前动作的destory属性设置为false，表示不立即销毁，
	//将索引start加1，判断是否需要重复执行动作序列或者执行完毕，
	//并根据情况设置destory属性为true以及调用回调接口的SSActionEvent()方法。
	public void SSActionEvent (SSAction source, SSActionEventType events = SSActionEventType.Competeted, int intParam = 0, string strParam = null, Object objectParam = null)
	{
		source.destory = false;
		this.start++;
		if (this.start >= sequence.Count) {
			this.start = 0;
			if (repeat > 0) repeat--;
			if (repeat == 0) {
				this.destory = true;
				this.callback.SSActionEvent (this); }
		}
	}

	//重写了基类的Start方法，
	//在该方法中，遍历动作序列sequence，
	//并为每个动作设置gameobject、transform和callback属性，最后调用各个动作的Start方法。
	// Use this for initialization
	public override void Start () {
		foreach (SSAction action in sequence) {
			action.gameobject = this.gameobject;
			action.transform = this.transform;
			action.callback = this;
			action.Start ();
		}
	}

	//遍历动作序列sequence，并且销毁每一个动作
	void OnDestory() {
		foreach(SSAction action in sequence){
			Destroy(action);
		}
	}
}

