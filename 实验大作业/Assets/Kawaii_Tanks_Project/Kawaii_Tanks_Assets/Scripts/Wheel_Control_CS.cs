using UnityEngine;
using System.Collections;

#if UNITY_ANDROID || UNITY_IPHONE
using UnityStandardAssets.CrossPlatformInput;
#endif

//用于车轮控制
// This script must be attached to "MainBody".
namespace ChobiAssets.KTP
{
	
	public class Wheel_Control_CS : MonoBehaviour
	{
		[ Header ("Driving settings")]
		[ Tooltip ("Torque added to each wheel.")] public float wheelTorque = 3000.0f; // Reference to "Wheel_Rotate".
		[ Tooltip ("Maximum Speed (Meter per Second)")] public float maxSpeed = 7.0f; // Reference to "Wheel_Rotate".
		[ Tooltip ("Rate for ease of turning."), Range (0.0f, 2.0f)] public float turnClamp = 0.8f;
		[ Tooltip ("'Solver Iteration Count' of all the rigidbodies in this tank.")] public int solverIterationCount = 7;

		// Reference to "Wheel_Rotate".
		[HideInInspector] public float leftRate;
		[HideInInspector] public float rightRate;

		Rigidbody thisRigidbody;
		bool isParkingBrake = false;
		float lagCount;
		float speedStep;
		float autoParkingBrakeVelocity = 0.5f;
		float autoParkingBrakeLag = 0.5f;

		ID_Control_CS idScript;

		/* for reducing Calls.
		Wheel_Rotate_CS[] rotateScripts;
		*/

		//这是 MonoBehaviour 类的一个生命周期方法，在游戏对象被加载时调用
		//Awake() 方法用于执行一些初始化操作
		//它设置了游戏对象的层级、获取 Rigidbody 组件，并设置刚体的迭代次数
		void Awake ()
		{
			this.gameObject.layer = 11; // Layer11 >> for MainBody.
			thisRigidbody = GetComponent < Rigidbody > ();
			thisRigidbody.solverIterations = solverIterationCount;
			/* for reducing Calls.
			rotateScripts = GetComponentsInChildren <Wheel_Rotate_CS> ();
			*/
		}

		//在每一帧更新时调用，Update() 方法用于根据输入更新车辆的行驶方向
		void Update ()
		{
			if (idScript.isPlayer) {
				#if UNITY_ANDROID || UNITY_IPHONE
				Mobile_Input ();
				#else
				Desktop_Input ();
				#endif
			}
		}

		//用于处理移动平台（Android 或 iPhone）上的输入。根据玩家的操作，它更新车辆的行驶方向
		#if UNITY_ANDROID || UNITY_IPHONE
		void Mobile_Input ()
		{
			if (CrossPlatformInputManager.GetButtonDown ("Up")) {
				speedStep += 0.5f;
				speedStep = Mathf.Clamp (speedStep, -1.0f, 1.0f);
			} else if (CrossPlatformInputManager.GetButtonDown ("Down")) {
				speedStep -= 0.5f;
				speedStep = Mathf.Clamp (speedStep, -1.0f, 1.0f);
			}
			float vertical = speedStep;
			float horizontal = 0.0f;
			if (CrossPlatformInputManager.GetButton ("Left")) {
				horizontal = Mathf.Lerp (-turnClamp, -1.0f, Mathf.Abs (vertical / 1.0f));
			} else if (CrossPlatformInputManager.GetButton ("Right")) {
				horizontal = Mathf.Lerp (turnClamp, 1.0f, Mathf.Abs (vertical / 1.0f));
			}
			if (vertical < 0.0f) {
				horizontal = -horizontal; // like a brake-turn.
			}
			leftRate = Mathf.Clamp (-vertical - horizontal, -1.0f, 1.0f);
			rightRate = Mathf.Clamp (vertical - horizontal, -1.0f, 1.0f);
		}
		#else

		//用于处理桌面平台上的输入。根据玩家的操作，它更新车辆的行驶方向
		void Desktop_Input ()
		{
			if (Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.W)) {
				speedStep += 0.5f;
				speedStep = Mathf.Clamp (speedStep, -1.0f, 1.0f);
			} else if (Input.GetKeyDown (KeyCode.DownArrow) || Input.GetKeyDown (KeyCode.S)) {
				speedStep -= 0.5f;
				speedStep = Mathf.Clamp (speedStep, -1.0f, 1.0f);
			} else if (Input.GetKeyDown (KeyCode.X)) {
				speedStep = 0.0f;
			}
			float vertical = speedStep;
			float horizontal = Input.GetAxis ("Horizontal");
			float clamp = Mathf.Lerp (turnClamp, 1.0f, Mathf.Abs (vertical / 1.0f));
			horizontal = Mathf.Clamp (horizontal, -clamp, clamp);
			if (vertical < 0.0f) {
				horizontal = -horizontal; // like a brake-turn.
			}
			leftRate = Mathf.Clamp (-vertical - horizontal, -1.0f, 1.0f);
			rightRate = Mathf.Clamp (vertical - horizontal, -1.0f, 1.0f);
		}
		#endif

		//在固定的时间间隔内调用，FixedUpdate() 方法用于更新车辆的物理属性，例如速度、角速度和停车制动
		void FixedUpdate ()
		{
			// Auto Parking Brake using 'RigidbodyConstraints'.
			if (leftRate == 0.0f && rightRate == 0.0f) {
				float velocityMag = thisRigidbody.velocity.magnitude;
				float angularVelocityMag = thisRigidbody.angularVelocity.magnitude;
				if (isParkingBrake == false) {
					if (velocityMag < autoParkingBrakeVelocity && angularVelocityMag < autoParkingBrakeVelocity) {
						lagCount += Time.fixedDeltaTime;
						if (lagCount > autoParkingBrakeLag) {
							isParkingBrake = true;
							thisRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;
						}
					}
				} else {
					if (velocityMag > autoParkingBrakeVelocity || angularVelocityMag > autoParkingBrakeVelocity) {
						isParkingBrake = false;
						thisRigidbody.constraints = RigidbodyConstraints.None;
						lagCount = 0.0f;
					}
				}
			} else {
				isParkingBrake = false;
				thisRigidbody.constraints = RigidbodyConstraints.None;
				lagCount = 0.0f;
			}
			/* for reducing Calls.
			for (int i = 0; i < rotateScripts.Length; i++) {
				rotateScripts [i].FixedUpdate_Me ();
			}
			*/
		}

		//在游戏对象被销毁时调用，Destroy() 方法用于处理游戏对象被破坏时的逻辑
		//它会禁用刚体的约束，并在一段时间后销毁游戏对象
		void Destroy ()
		{ // Called from "Damage_Control_CS".
			StartCoroutine ("Disable_Constraints");
		}

		//这个协程方法用于在一段时间后禁用刚体的约束，并最终销毁脚本所附加的游戏对象
		IEnumerator Disable_Constraints ()
		{
			// Disable constraints of MainBody's rigidbody.
			yield return new WaitForFixedUpdate (); // This wait is required for PhysX.
			thisRigidbody.constraints = RigidbodyConstraints.None;
			Destroy (this);
		}

		//这个函数用于获取与脚本交互的 ID_Control_CS 脚本的引用
		void Get_ID_Script (ID_Control_CS tempScript)
		{
			idScript = tempScript;
		}

		//这个函数用于根据游戏是否暂停来启用或禁用脚本的更新
		void Pause (bool isPaused)
		{ // Called from "Game_Controller_CS".
			this.enabled = !isPaused;
		}

	}

}
