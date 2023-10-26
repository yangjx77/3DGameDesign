using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//动作管理基类
//将许多个动作整合到一起，然后顺序调用
public class SSActionManager : MonoBehaviour {

	//为一个字典，用于存储所有的SSAction对象，以其实例ID为键。
	private Dictionary <int, SSAction> actions = new Dictionary <int, SSAction> ();
	//一个列表，用于存储等待添加的SSAction对象
	private List <SSAction> waitingAdd = new List<SSAction> ();
	//一个列表，用于存储等待删除的SSAction对象 
	private List<int> waitingDelete = new List<int> ();

	// Update is called once per frame
	protected void Update () {
		//首先将所有等待添加的SSAction对象添加到actions字典中
		foreach (SSAction ac in waitingAdd){
			actions[ac.GetInstanceID ()] = ac;
		}

		//清空等待添加的列表
		waitingAdd.Clear ();

		//遍历actions字典中的每一个SSAction对象
		foreach (KeyValuePair <int, SSAction> kv in actions) {
			SSAction ac = kv.Value;
			//如果action的destory属性为true时，将该对象添加到等待删除的队列中
			if (ac.destory) { 
				waitingDelete.Add(ac.GetInstanceID()); // release action
			}
			//如果action的enable为true时，将调用Update函数更新该action 
			else if (ac.enable) { 
				ac.Update (); // update action
			}
		}

		//遍历等待删除的对象列表，将所有的对象销毁，并且清空该等待删除的列表
		foreach (int key in waitingDelete) {
			SSAction ac = actions[key]; 
			actions.Remove(key); 
			Object.Destroy(ac);
		}
		waitingDelete.Clear ();
	}

	//为外部的一个接口函数，将参数进行绑定
	public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager) {
		action.gameobject = gameobject;
		action.transform = gameobject.transform;
		action.callback = manager;

		//将该动作添加到等待添加的对象列表中
		//同时调用对象的Start方法来进行初始化
		waitingAdd.Add (action);
		action.Start ();
	}


	// Use this for initialization
	protected void Start () {
		
	}
}
