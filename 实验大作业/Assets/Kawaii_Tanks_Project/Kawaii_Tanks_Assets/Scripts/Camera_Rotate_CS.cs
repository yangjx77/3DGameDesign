using UnityEngine;
using System.Collections;

#if UNITY_ANDROID || UNITY_IPHONE
using UnityStandardAssets.CrossPlatformInput;
#endif

//实现了相机的旋转控制
// This script must be attached to "Camera_Pivot".
namespace ChobiAssets.KTP
{
	
	public class Camera_Rotate_CS : MonoBehaviour
	{

		//用于存储组件信息
		Transform thisTransform;
		//存储鼠标位置
		Vector2 previousMousePos;
		float angY;   //绕Y轴的旋转角度
		float angZ;   //绕Z轴的旋转角度
		float targetAng_Y ;   //绕Y轴的目标旋转角度

		#if UNITY_ANDROID || UNITY_IPHONE
		bool isButtonDown = false;    //标记是否按下了相机控制的按钮
		int fingerID;                 //用于存储手指的ID
		#endif

		ID_Control_CS idScript;       //存储脚本的引用

		void Awake ()
		{
			//变量绑定
			thisTransform = transform;
			angY = thisTransform.eulerAngles.y;
			targetAng_Y = angY;
			angZ = thisTransform.eulerAngles.z;
		}

		void Update ()
		{
			//变量更新
			if (idScript.isPlayer) {
				#if UNITY_ANDROID || UNITY_IPHONE
				Mobile_Input () ;
				#else
				Desktop_Input ();
				#endif
			}
		}

		#if UNITY_ANDROID || UNITY_IPHONE
		//处理移动平台的相机输入
		void Mobile_Input ()
		{
			if (CrossPlatformInputManager.GetButtonDown ("Camera_Press")) {
				isButtonDown = true ;
				#if UNITY_EDITOR
				previousMousePos = Input.mousePosition;
				#else
				fingerID = Input.touches.Length - 1;
				previousMousePos = Input.touches [fingerID].position;
				#endif
				return;
			}
			if (isButtonDown && CrossPlatformInputManager.GetButton ("Camera_Press")) {
				#if UNITY_EDITOR
				Vector3 currentMousePos = Input.mousePosition;
				#else
				Vector3 currentMousePos = Input.touches [fingerID].position;
				#endif
				float horizontal = (currentMousePos.x - previousMousePos.x);
				//float vertical = (currentMousePos.y - previousMousePos.y);
				targetAng_Y += horizontal * 0.5f;
				angY = targetAng_Y;
				//angZ -= vertical * 0.2f;
				previousMousePos = currentMousePos ;
			} else if (CrossPlatformInputManager.GetButtonUp ("Camera_Press")) {
				isButtonDown = false;
			}
			angY = Mathf.MoveTowardsAngle (angY, targetAng_Y, 180.0f * Time.deltaTime);
			thisTransform.rotation = Quaternion.Euler (0.0f, angY, angZ);
		}
		#else

		//处理平台上的相机输入
		void Desktop_Input ()
		{
			Mouse_Input_Drag (true);
		}

		//处理鼠标的拖拽输入
		void Mouse_Input_Drag (bool isFreeAiming)
		{
			if (idScript.aimButton == false) {
				if (idScript.dragButtonDown) {
					previousMousePos = Input.mousePosition;
				}
				if (idScript.dragButton) {
					float horizontal = (Input.mousePosition.x - previousMousePos.x) * 0.1f;
					targetAng_Y += horizontal * 3.0f;
					angY = targetAng_Y;
					//float vertical = (Input.mousePosition.y - previousMousePos.y) * 0.1f;
					//angZ -= vertical * 2.0f;
					previousMousePos = Input.mousePosition;
				}
			}
			angY = Mathf.MoveTowardsAngle (angY, targetAng_Y, 180.0f * Time.deltaTime);
			thisTransform.rotation = Quaternion.Euler (0.0f, angY, angZ);
		}
		#endif

		//获取脚本的引用
		void Get_ID_Script (ID_Control_CS tempScript)
		{
			idScript = tempScript;
		}

		//用于暂停相机的旋转效果
		void Pause (bool isPaused)
		{ // Called from "Game_Controller_CS".
			this.enabled = !isPaused;
		}

	}

}
