using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//枚举了动作事件的类型：开始和完成
public enum SSActionEventType:int { Started, Competeted }

//一个接口，用于在动作完成时进行回调
public interface ISSActionCallback
{
	//source为一个SSAction类型的参数，表示触发事件的动作对象
	//events为一个SSActionEventType类型的参数，表示事件类型，默认为Competeted
	void SSActionEvent(SSAction source, 
		SSActionEventType events = SSActionEventType.Competeted,
		int intParam = 0 , 
		string strParam = null, 
		Object objectParam = null);
}

