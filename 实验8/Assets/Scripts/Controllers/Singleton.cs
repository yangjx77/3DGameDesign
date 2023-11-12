using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用于单实例化飞碟工厂
//<T>表示该类是一个泛型类，可以用任何类型来实例化
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	//声明一个静态的受保护的字段
	//用于存储实例化的对象
	protected static T instance;

	//公共的只读属性
	//通过该属性获取单例对象
	public static T Instance {  
		get {  
			//首先检查instance是否为空
			if (instance == null) { 
				//如果为空，就使用FindObjectOfType查找场景中第一个符合类型T的对象
				//并且将其赋值给instance
				instance = (T)FindObjectOfType (typeof(T));  
				//如果instance仍然为空，就输出错误日志，提示需要往场景中添加类型T的实例
				if (instance == null) {  
					Debug.LogError ("An instance of " + typeof(T) +
					" is needed in the scene, but there is none.");  
				}  
			}  
			//返回唯一的单实例对象
			return instance;  
		}  
	}
}
