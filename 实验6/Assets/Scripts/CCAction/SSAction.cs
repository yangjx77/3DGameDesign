using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//动作基类
//将平移、旋转、缩放抽象出一个动作基类
public class SSAction : ScriptableObject {

	//用于标记该动作是否被启用
	public bool enable = true;
	//用于标记该动作是否被摧毁
	public bool destory = false;

	//表示该动作作用的对象
	public GameObject gameobject { get; set; }
	//表示该动作所作用的对象的变换组件
	public Transform transform { get; set; }
	//表示该动作完成后的回调接口
	public ISSActionCallback callback { get; set; }

	//构造函数
	protected SSAction () {}

	// Use this for initialization
	public virtual void Start () {
		//抛出异常，表示在子类中必须完成相应的实现
		throw new System.NotImplementedException ();
	}
	
	// Update is called once per frame
	public virtual void Update () {
		//抛出异常，表示在子类中必须完成相应的实现
		throw new System.NotImplementedException ();
	}
		
}
