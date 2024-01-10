using UnityEngine;
using System.Collections;

// This script must be attached to all the Apparent Wheels.
//用于轮胎的同步，必须附加到所有轮胎上
namespace ChobiAssets.KTP
{
	
	public class Wheel_Sync_CS : MonoBehaviour
	{
		[ Header ("Wheel Synchronizing settings")]   //用于在Inspector面板中显示一个标题，用来标识轮胎同步设置
		[ Tooltip ("Set the RoadWheel to synchronize with.")] public Transform referenceWheel; //用于在Inspector面板中显示一个提示，并允许用户指定要同步的参考轮胎
		[ Tooltip ("Offset value for the size of this wheel.")] public float radiusOffset = 0.0f;  //用于在Inspector面板中显示一个提示，并允许用户指定轮胎大小的偏移值

		Transform thisTransform; //当前轮胎的 Transform 组件
		bool isLeft;             //一个布尔值，用于表示轮胎是否位于左侧
		float previousAng;       //用于存储上一帧的参考轮胎的旋转角度
		float radiusRate;        //用于存储参考轮胎与当前轮胎半径之间的比率
 
		//获取当前轮胎的 Transform 组件
		//根据轮胎在局部坐标系的位置判断轮胎是否在左侧
		//检查并查找参考轮胎。如果在 Inspector 面板中没有指定参考轮胎，将通过查找父物体的 Track_Scroll_CS 脚本来自动找到适合的参考轮胎
		//计算参考轮胎与当前轮胎半径之间的比率
		void Awake ()
		{
			thisTransform = transform;
			// Set direction.
			if (transform.localPosition.y > 0.0f) {
				isLeft = true;
			} else {
				isLeft = false;
			}
			// Check and Find referenceWheel.
			if (referenceWheel == null) {
				Find_Reference_Wheel ();
			}
			// Calculate radiusRate.
			MeshFilter referenceMeshFilter = referenceWheel.GetComponent <MeshFilter> ();
			if (referenceMeshFilter) {
				float thisRadius = GetComponent <MeshFilter> ().mesh.bounds.extents.z + radiusOffset;
				float referenceRadius = referenceMeshFilter.mesh.bounds.extents.z;
				if (referenceRadius > 0.0f && thisRadius > 0.0f) {
					radiusRate = referenceRadius / thisRadius;
				}
			}
		}

		//在父物体的所有 Track_Scroll_CS 脚本中查找适合的参考轮胎
		//如果找到匹配的参考轮胎，将其赋值给 referenceWheel 变量
		//如果未找到匹配的参考轮胎，将输出错误信息，并销毁脚本自身
		void Find_Reference_Wheel ()
		{
			Track_Scroll_CS[] scrollScripts = thisTransform.parent.parent.GetComponentsInChildren <Track_Scroll_CS> ();
			foreach (Track_Scroll_CS scrollScript in scrollScripts) {
				if ((isLeft && scrollScript.referenceWheel.localPosition.y > 0.0f) || (isLeft == false && scrollScript.referenceWheel.localPosition.y < 0.0f)) {
					referenceWheel = scrollScript.referenceWheel;
					break;
				}
			}
			if (referenceWheel == null) {
				Debug.LogError ("Reference Wheel is not assigned in " + this.name);
				Destroy (this);
			}
		}

		//如果有指定的参考轮胎（referenceWheel 不为空）：
		//获取当前参考轮胎的旋转角度
		//计算当前旋转角度与上一帧旋转角度之间的差值
		//根据参考轮胎与当前轮胎半径比率，调整当前轮胎的旋转角度
		//更新上一帧的旋转角度为当前旋转角度
		void Update ()
		{
			if (referenceWheel) {
				float currentAng = referenceWheel.localEulerAngles.y;
				float deltaAng = Mathf.DeltaAngle (currentAng, previousAng);
				thisTransform.localEulerAngles = new Vector3 (thisTransform.localEulerAngles.x, thisTransform.localEulerAngles.y - (radiusRate * deltaAng), thisTransform.localEulerAngles.z);
				previousAng = currentAng;
			}
		}

		//当游戏暂停时调用，根据传入的布尔值参数来启用或禁用脚本的更新
		void Pause (bool isPaused)
		{ // Called from "Game_Controller_CS".
			this.enabled = !isPaused;
		}

	}

}
