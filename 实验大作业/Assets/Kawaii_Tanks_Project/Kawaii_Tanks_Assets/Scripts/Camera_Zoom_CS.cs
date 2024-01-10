using UnityEngine;
using System.Collections;

#if UNITY_ANDROID || UNITY_IPHONE
using UnityStandardAssets.CrossPlatformInput;
#endif

// This script must be attached to "Main Camera".
// (Note.) Main Camera must be placed on X Aixs of "Camera_Pivot".
//用于控制摄像机的缩放
namespace ChobiAssets.KTP
{
	
	public class Camera_Zoom_CS : MonoBehaviour
	{

		Transform thisTransform;
		Transform parentTransform;
		Transform rootTransform;
		Camera thisCamera;
		AudioListener thisAudioListener;
		float posX;
		float targetPosX;
		int layerMask = ~((1 << 10) + (1 << 2)); // Layer 2 = Ignore Ray, Layer 10 = Ignore All.
		float storedPosX;
		bool autoZoomFlag;
		float hitCount;

		#if UNITY_ANDROID || UNITY_IPHONE
		bool isButtonDown = false;
		Vector2 previousMousePos;
		int fingerID;
		#endif

		public float speed = 30.0f;

		ID_Control_CS idScript;

		//初始化一些变量和组件，包括获取摄像机组件和音频监听器组件，设置初始位置
		void Awake ()
		{
			this.tag = "MainCamera";
			thisCamera = GetComponent < Camera > ();
			thisCamera.enabled = false;
			thisAudioListener = GetComponent < AudioListener > ();
			thisAudioListener.enabled = false;
			thisTransform = transform;
			parentTransform = thisTransform.parent;
			rootTransform = thisTransform.root;
			posX = transform.localPosition.x;
			targetPosX = posX;
		}

		//在每一帧更新时调用
        //检测玩家输入并根据输入控制摄像机的缩放
		//调用Cast_Ray()函数检测摄像机是否需要自动缩放
		void Update ()
		{
			if (idScript.isPlayer) {
				#if UNITY_ANDROID || UNITY_IPHONE
				if (CrossPlatformInputManager.GetButtonDown ("Zoom_Press")) {
					isButtonDown = true ;
				#if UNITY_EDITOR
					previousMousePos = Input.mousePosition;
				#else
					fingerID = Input.touches.Length - 1;
					previousMousePos = Input.touches [fingerID].position;
				#endif
					return;
				}
				if (isButtonDown && CrossPlatformInputManager.GetButton ("Zoom_Press")) {
				#if UNITY_EDITOR
					Vector3 currentMousePos = Input.mousePosition;
				#else
					Vector3 currentMousePos = Input.touches [fingerID].position;
				#endif
					float vertical = (currentMousePos.y - previousMousePos.y);
					targetPosX += vertical * 0.1f;
					targetPosX = Mathf.Clamp (targetPosX, 3.0f, 20.0f);
					previousMousePos = currentMousePos ;
				} else if (CrossPlatformInputManager.GetButtonUp ("Zoom_Press")) {
					isButtonDown = false;
				}
				#else
				float axis = Input.GetAxis ("Mouse ScrollWheel");
				if (axis != 0) {
					#if UNITY_WEBGL
					targetPosX -= axis * 10.0f;
					#else
					targetPosX -= axis * 30.0f;
					#endif
					targetPosX = Mathf.Clamp (targetPosX, 3.0f, 20.0f);
				}
				#endif
				if (posX != targetPosX) {
					posX = Mathf.MoveTowards (posX, targetPosX, speed * Time.deltaTime);
					thisTransform.localPosition = new Vector3 (posX, thisTransform.localPosition.y, thisTransform.localPosition.z);
				} else {
					Cast_Ray ();
				}
			}
		}

		//发射射线，检测摄像机与场景中的物体之间是否有碰撞
		//如果有碰撞，根据碰撞距离调整摄像机的缩放位置
		//如果没有碰撞，恢复摄像机的初始位置
		void Cast_Ray () {
			Ray ray = new Ray (parentTransform.position, thisTransform.position - parentTransform.position);
			RaycastHit[] raycastHits;
			raycastHits = Physics.SphereCastAll (ray, 0.5f, thisTransform.localPosition.x + 1.0f, layerMask);
			foreach (RaycastHit raycastHit in raycastHits) {
				if (raycastHit.transform.root != rootTransform) { // not itself.
					hitCount += Time.deltaTime;
					if (hitCount > 0.5f) {
						hitCount = 0.0f;
						if (autoZoomFlag == false) {
							autoZoomFlag = true;
							storedPosX = posX;
							targetPosX = raycastHit.distance;
							targetPosX = Mathf.Clamp (targetPosX, 3.0f, 20.0f);
						} else {
							if (targetPosX > raycastHit.distance) {
								targetPosX = raycastHit.distance;
								targetPosX = Mathf.Clamp (targetPosX, 3.0f, 20.0f);
							}
						}
					}
					return;
				}
			}
			hitCount = 0.0f;
			if (autoZoomFlag) {
				autoZoomFlag = false;
				targetPosX = storedPosX;
			}
		}

		//从ID_Control_CS脚本中获取对应的组件引用
		//根据获取的组件引用决定是否启用摄像机和音频监听器
		void Get_ID_Script (ID_Control_CS tempScript)
		{
			idScript = tempScript;
			if (idScript.isPlayer) {
				thisAudioListener.enabled = true;
				thisCamera.enabled = true;
			}
			idScript.mainCamScript = this;
		}

		//从"Game_Controller_CS"脚本调用
		//根据传入的参数决定是否暂停摄像机的更新
		void Pause (bool isPaused)
		{ // Called from "Game_Controller_CS".
			this.enabled = !isPaused;
		}

		//根据传入的参数决定是否启用摄像机和音频监听器
		public void Switch_Player (bool isPlayer)
		{
			thisAudioListener.enabled = isPlayer;
			thisCamera.enabled = isPlayer;
		}

	}

}
