using UnityEngine;
using System.Collections;

//当游戏运行在Android或iPhone平台时，在游戏对象被加载时立即销毁它
namespace ChobiAssets.KTP
{
	
	public class Tutorial_Text_CS : MonoBehaviour
	{
		#if UNITY_ANDROID || UNITY_IPHONE
		void Awake ()
		{
			Destroy (this.gameObject);
		}
		#endif
	}

}
