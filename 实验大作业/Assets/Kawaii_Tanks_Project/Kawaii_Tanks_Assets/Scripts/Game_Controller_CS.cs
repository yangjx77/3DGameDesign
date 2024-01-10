using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_ANDROID || UNITY_IPHONE
using UnityStandardAssets.CrossPlatformInput;
#endif

//用于整体的游戏控制
namespace ChobiAssets.KTP
{
	[ System.Serializable]
	public class TankProp
	{
		public Transform bodyTransform;
		public Rigidbody bodyRigidbody;
	}

	public class Game_Controller_CS : MonoBehaviour {

		[Header ("Game settings")]
		[Tooltip ("Interval for physics calculations.")] public float fixedTimestep = 0.03f;
		#if UNITY_ANDROID || UNITY_IPHONE
		[Tooltip ("Maximum resolution for mobile device.")] public int maxResolution = 720;
		#endif

		[Header ("Stage settings")]
		[Tooltip ("Set the prefab of 'Touch_Controls'.")] public GameObject touchControlsPrefab;

		List <ID_Control_CS> tankList;
		int currentID = 0;
		bool isPaused;

		//在脚本启动时调用
		//设置物理计算的固定时间步长（fixedTimestep）
		//初始化坦克列表（tankList）
		//如果是在移动设备上（UNITY_ANDROID || UNITY_IPHONE），实例化触摸控制器（touchControlsPrefab）
		//根据移动设备的最大分辨率（maxResolution）调整屏幕分辨率
		//设置游戏对象的标签为"GameController"
		//设置层之间的碰撞关系，包括重置和忽略碰撞
		void Awake ()
		{
			Time.fixedDeltaTime = fixedTimestep;
			tankList = new List <ID_Control_CS> ();
			#if UNITY_ANDROID || UNITY_IPHONE
			if (touchControlsPrefab) {
				Instantiate (touchControlsPrefab);
			}
			float screenRate = (float)maxResolution / Screen.height;
			if (screenRate > 1.0f) {
				screenRate = 1.0f;
			}
			int width = (int)(Screen.width * screenRate);
			int height = (int)(Screen.height * screenRate);
			Screen.SetResolution(width, height, true);
			#endif
			this.tag = "GameController";
			/*
			Layer Collision Settings.
			Layer9 >> for wheels.
			Layer10 >> for Suspensions.
			Layer11 >> for MainBody.
			*/
			for (int i = 0; i <= 11; i++) {
				Physics.IgnoreLayerCollision (9, i, false); // Reset settings.
				Physics.IgnoreLayerCollision (11, i, false); // Reset settings.
			}
			Physics.IgnoreLayerCollision (9, 9, true); // Wheels do not collide with each other.
			Physics.IgnoreLayerCollision (9, 11, true); // Wheels do not collide with MainBody.
			for (int i = 0; i <= 11; i++) {
				Physics.IgnoreLayerCollision (10, i, true); // Suspensions do not collide with anything.
			}
		}

		//接收ID_Control_CS脚本发送的ID
		//存储坦克的组件信息（bodyTransform和bodyRigidbody）
		//将坦克添加到坦克列表（tankList）中
		//如果ID为0，表示当前操作的是玩家坦克
		public void Receive_ID (ID_Control_CS idScript)
		{
			// Store the components in the lists.
			TankProp tankProp = new TankProp ();
			tankProp.bodyRigidbody = idScript.GetComponentInChildren <Rigidbody> ();
			tankProp.bodyTransform = tankProp.bodyRigidbody.transform;
			idScript.storedTankProp = tankProp;
			// Add the tank to tankList.
			tankList.Add (idScript);
			// Send currentID (0).
			if (idScript.id == 0) {
				idScript.isPlayer = true;
			} else {
				idScript.isPlayer = false;
			}
		}

		//在每一帧更新中调用
		//切换当前可操作的坦克
		//如果按下Tab键（或者在移动设备上按下Switch按钮），将当前ID加1，并确保不超过坦克列表的长度
		//处理退出游戏的逻辑
		//处理重新加载场景的逻辑
		//处理暂停游戏的逻辑
		void Update ()
		{
			// Switch operable tank.
			#if UNITY_ANDROID || UNITY_IPHONE
			if (CrossPlatformInputManager.GetButtonDown ("Switch")) {
			#else
			if (Input.GetKeyDown (KeyCode.Tab)) {
			#endif
				currentID += 1;
				if (currentID > tankList.Count - 1) {
					currentID = 0;
				}
				for (int i = 0; i < tankList.Count; i++) {
					tankList [i].Get_Current_ID (currentID);
				}
			}
			// Quit
			if (Input.GetKeyDown (KeyCode.Escape)) {
				Application.Quit ();
			}
			// Reload the scene.
			#if UNITY_ANDROID || UNITY_IPHONE
			if (CrossPlatformInputManager.GetButtonDown ("Restart")) {
			#else
			if (Input.GetKeyDown (KeyCode.Backspace)) {
			#endif
				if (isPaused) {
					Pause ();
				}
				SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
			}
			// Pause.
			#if UNITY_ANDROID || UNITY_IPHONE
			if (CrossPlatformInputManager.GetButtonDown ("Pause")) {
			#else
			if (Input.GetKeyDown ("p")) {
			#endif
				Pause ();
			}
		}

		//暂停游戏
		//切换isPaused的值
		//根据isPaused设置时间缩放（timeScale）
		//向所有坦克发送暂停消息
		void Pause ()
		{
			isPaused = !isPaused;
			// Set timeScale and text.
			if (isPaused) {
				Time.timeScale = 0.0f;
			} else {
				Time.timeScale = 1.0f;
			}
			// Send Message to all the tanks.
			ID_Control_CS[] idScripts = FindObjectsOfType <ID_Control_CS> ();
			foreach (ID_Control_CS idScript in idScripts) {
				idScript.BroadcastMessage ("Pause", isPaused, SendMessageOptions.DontRequireReceiver);
			}
		}

	}

}
