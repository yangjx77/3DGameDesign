using UnityEngine;
using System.Collections;

//用于在一定时间后销毁游戏对象
namespace ChobiAssets.KTP
{
	public class Delete_Timer_CS : MonoBehaviour
	{

		[ Header ("Life time settings")]
		[ Tooltip ("Life time of this gameobject. (Sec)")] public float lifeTime = 2.0f;

		//在脚本启动时调用
        //使用Destroy函数在一定时间后销毁当前游戏对象
		//销毁的延迟时间由lifeTime变量指定
		void Awake ()
		{
			Destroy (this.gameObject, lifeTime);
		}

	}

}
