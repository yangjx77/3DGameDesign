using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

// This script must be attached to the top object of the tank.
//用于管理坦克ID和相关组件
namespace ChobiAssets.KTP
{

	public class ID_Control_CS : MonoBehaviour
	{

		[Header ("ID settings")]
		[Tooltip ("ID number")] public int id = 0;

		[HideInInspector] public bool isPlayer; // Referred to from child objects.
		[HideInInspector] public Game_Controller_CS controllerScript;
		[HideInInspector] public TankProp storedTankProp; // Set by "Game_Controller_CS".

		[HideInInspector] public Turret_Control_CS turretScript;
		[HideInInspector] public Camera_Zoom_CS mainCamScript;
		[HideInInspector] public GunCamera_Control_CS gunCamScript;


		//在脚本启动时调用（不要改为"Awake()"）
		//查找标签为"GameController"的游戏对象，并获取其上的Game_Controller_CS脚本的引用（controllerScript）
		//如果找到了controllerScript，则调用其Receive_ID()方法，将当前脚本（ID_Control_CS）的引用传递给controllerScript
		//如果未找到controllerScript，则输出错误信息
		//向所有子对象发送"Get_ID_Script"消息，将当前脚本（ID_Control_CS）的引用传递给它们
		void Start ()
		{ // Do not change to "Awake ()".
			// Send this reference to the "Game_Controller" in the scene.
			GameObject gameController = GameObject.FindGameObjectWithTag ("GameController");
			if (gameController) {
				controllerScript = gameController.GetComponent <Game_Controller_CS> ();
			}
			if (controllerScript) {
				controllerScript.Receive_ID (this);
			} else {
				Debug.LogError ("There is no 'Game_Controller' in the scene.");
			}
			// Broadcast this reference.
			BroadcastMessage ("Get_ID_Script", this, SendMessageOptions.DontRequireReceiver);
		}

		#if !UNITY_ANDROID && !UNITY_IPHONE
		[HideInInspector] public bool aimButton;
		[HideInInspector] public bool aimButtonDown;
		[HideInInspector] public bool aimButtonUp;
		[HideInInspector] public bool dragButton;
		[HideInInspector] public bool dragButtonDown;
		[HideInInspector] public bool fireButton;

		//仅在非移动设备平台（!UNITY_ANDROID && !UNITY_IPHONE）上调用
		//如果当前坦克是玩家控制的，则根据按键状态更新相关的布尔变量：
		//aimButton：按下并持续按住空格键。
		//aimButtonDown：刚刚按下空格键。
		//aimButtonUp：释放空格键。
		//dragButton：按住鼠标右键。
		//dragButtonDown：刚刚按下鼠标右键。
		//fireButton：按住鼠标左键。
		void Update ()
		{
			if (isPlayer) {
				aimButton = Input.GetKey (KeyCode.Space);
				aimButtonDown = Input.GetKeyDown (KeyCode.Space);
				aimButtonUp = Input.GetKeyUp (KeyCode.Space);
				dragButton = Input.GetMouseButton (1);
				dragButtonDown = Input.GetMouseButtonDown (1);
				fireButton = Input.GetMouseButton (0);
			}
		}
		#endif

		//从"Damage_Control_CS"脚本中调用，用于销毁坦克
		//将当前游戏对象的标签设置为"Finish"
		void Destroy ()
		{ // Called from "Damage_Control_CS".
			gameObject.tag = "Finish";
		}

		//从"Game_Controller_CS"脚本中调用，用于获取当前操作的坦克ID
		//如果当前坦克的ID与传入的currentID相同，则将isPlayer设置为true，表示当前坦克是玩家控制的
		//否则，将isPlayer设置为false
		//调用turretScript、mainCamScript和gunCamScript的Switch_Player()方法，将isPlayer传递给它们，以切换坦克的控制状态
		public void Get_Current_ID (int currentID)
		{ // Called from "Game_Controller_CS".
			if (id == currentID) {
				isPlayer = true;
			} else {
				isPlayer = false;
			}
			// Call Switch_Player.
			turretScript.Switch_Player (isPlayer);
			mainCamScript.Switch_Player (isPlayer);
			gunCamScript.Switch_Player (isPlayer);
		}

	}

}
