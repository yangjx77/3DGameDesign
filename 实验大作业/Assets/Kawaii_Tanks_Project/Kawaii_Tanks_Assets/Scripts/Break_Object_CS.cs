using UnityEngine;
using System.Collections;

//用于实现游戏中游戏对象破碎的效果
namespace ChobiAssets.KTP
{
	
	public class Break_Object_CS : MonoBehaviour
	{
		//用于设置游戏对象破碎的参数
		[ Header ("Broken object settings")]
		[ Tooltip ("Prefab of the broken object.")] public GameObject brokenPrefab;    //破碎对象的预制体
		[ Tooltip ("Lag time for breaking. (Sec)")] public float lagTime = 1.0f;       //破碎对象的延迟时间

		Transform thisTransform;     //组件对象

		void Awake ()
		{
			thisTransform = transform;   //绑定组件对象
		}

		void OnJointBreak ()
		{
			//它使用协程来调用名为Broken的方法
			//在游戏中，当与当前对象连接的关节断开时，该方法会触发对象的破碎效果
			StartCoroutine ("Broken");
		}

		void OnTriggerEnter (Collider collider)
		{
			if (collider.isTrigger == false) {
				StartCoroutine ("Broken");
			}
		}

		//用于实现对象的破碎效果
		IEnumerator Broken ()
		{
			//首先等待一段时间lagTime
			yield return new WaitForSeconds (lagTime);
			if (brokenPrefab) {
				//然后在当前对象的位置和旋转处实例化破碎对象的预制体
				Instantiate (brokenPrefab, thisTransform.position, thisTransform.rotation);
			}
			//销毁当前对象
			Destroy (gameObject);
		}

	}

}
