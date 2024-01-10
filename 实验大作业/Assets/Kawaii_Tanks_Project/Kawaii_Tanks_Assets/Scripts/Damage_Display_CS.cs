using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//用于显示伤害文本
namespace ChobiAssets.KTP
{

	public class Damage_Display_CS : MonoBehaviour
	{
		
		[Header ("Display settings")]
		[Tooltip ("Upper offset for displaying the value.")] public float offset = 256.0f;
		[Tooltip ("Displaying time.")] public float time = 1.5f;

		Transform thisTransform;
		Text thisText;

		// Set by "Damage_Control_CS".
		[HideInInspector] public Transform targetTransform ;

		//在脚本启动时调用
        //获取当前游戏对象的Transform组件和Text组件的引用
		//将Text组件的enabled属性设置为false，初始状态下不显示文本
		void Awake ()
		{
			thisTransform = GetComponent <Transform> ();
			thisText = GetComponent <Text> ();
			thisText.enabled = false;
		}

		//从Damage_Control_CS脚本中调用
        //根据传入的耐久度(durability)和初始耐久度(initialDurability)设置文本的内容
		//调用Display协程开始显示文本
		public void Get_Damage (float durability, float initialDurability)
		{ // Called from "Damage_Control_CS".
			thisText.text = Mathf.Ceil (durability) + "/" + initialDurability;
			StartCoroutine ("Display");
		}

		//协程函数，用于在一定时间内显示文本
		//在开始显示文本前，记录当前的显示计数和文本颜色
		//在一定时间内逐渐减少文本的透明度，实现淡出效果
		//如果在显示过程中计数发生变化（表示有新的文本需要显示），则退出协程
		//如果目标位置(targetTransform)存在，则调用Set_Position()设置文本的位置
		//在显示完成后，重置显示计数和文本的enabled属性
		int displayingCount;
		IEnumerator Display ()
		{
			float count = 0.0f;
			displayingCount += 1;
			int myNum = displayingCount;
			Color currentColor = thisText.color;
			while (count < time) {
				if (myNum < displayingCount) {
					yield break;
				}
				if (targetTransform) {
					Set_Position ();
				} else {
					break;
				}
				currentColor.a = Mathf.Lerp (1.0f, 0.0f, count / time);
				thisText.color = currentColor;
				count += Time.deltaTime;
				yield return null;
			}
			displayingCount = 0;
			thisText.enabled = false;
		}

		//根据目标位置(targetTransform)设置文本的显示位置
		//通过计算目标位置与摄像机的距离，根据摄像机的视野和距离计算文本的显示位置
		//如果目标位置在摄像机前方，则启用文本的显示，并根据距离调整文本的垂直偏移量
		//如果目标位置在摄像机后方，则禁用文本的显示
		void Set_Position ()
		{
			float lodValue = 2.0f * Vector3.Distance (targetTransform.position, Camera.main.transform.position) * Mathf.Tan (Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
			Vector3 currentPos = Camera.main.WorldToScreenPoint (targetTransform.position);
			if (currentPos.z > 0.0f) { // In front of the camera.
				thisText.enabled = true;
				currentPos.z = 100.0f;
				currentPos.y += (5.0f / lodValue) * offset;
				thisTransform.position = currentPos;
				thisTransform.localScale = Vector3.one;
			} else { // Behind of the camera.
				thisText.enabled = false;
			}
		}

	}

}