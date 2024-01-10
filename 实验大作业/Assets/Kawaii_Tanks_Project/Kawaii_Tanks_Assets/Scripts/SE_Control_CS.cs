using UnityEngine;
using System.Collections;

// This script must be attached to "Engine_Sound".
//用于控制引擎声音
namespace ChobiAssets.KTP
{

	public class SE_Control_CS : MonoBehaviour
	{
		[ Header ("Engine Sound settings")]
		[ Tooltip ("Set the Left RoadWheel to synchronize with.")] public Rigidbody leftReferenceWheel; //用于与左侧路轮同步的刚体组件
		[ Tooltip ("Set the Left RoadWheel to synchronize with.")] public Rigidbody rightReferenceWheel;//用于与右侧路轮同步的刚体组件
		[ Tooltip ("Minimum Pitch")] public float minPitch = 1.0f;   //最小音调
		[ Tooltip ("Maximum Pitch")] public float maxPitch = 2.0f;   //最大音调
		[ Tooltip ("Minimum Volume")] public float minVolume = 0.1f;  //最小音量
		[ Tooltip ("Maximum Volume")] public float maxVolume = 0.3f;  //最大音量

		AudioSource thisAudioSource;
		float leftCircumference;
		float rightCircumference;
		float currentRate;
		const float DOUBLE_PI = Mathf.PI * 2.0f;
		float maxSpeed;

		//在脚本启动时调用
		//获取AudioSource组件的引用
		//检查AudioSource是否已附加到当前游戏对象，如果没有，则输出错误信息并销毁脚本
		//设置AudioSource的循环播放属性为true
		//设置AudioSource的音量为0.0
		//播放AudioSource
		//检查并查找左右路轮的参考刚体
		//计算左右路轮的周长
		void Awake ()
		{
			thisAudioSource = GetComponent <AudioSource> ();
			if (thisAudioSource == null) {
				Debug.LogError ("AudioSource is not attached to" + this.name);
				Destroy (this);
			}
			thisAudioSource.loop = true;
			thisAudioSource.volume = 0.0f;
			thisAudioSource.Play ();
			// Check and Find referenceWheel.
			if (leftReferenceWheel == null || rightReferenceWheel == null) {
				Find_Reference_Wheels ();
			}
			// Calculate Circumferences.
			leftCircumference = DOUBLE_PI * leftReferenceWheel.GetComponent <SphereCollider> ().radius;
			rightCircumference = DOUBLE_PI * rightReferenceWheel.GetComponent <SphereCollider> ().radius;
		}

		//在脚本启动时调用
		//获取父对象的Wheel_Control_CS脚本的引用，并获取其maxSpeed属性的值，赋给maxSpeed变量
		void Start ()
		{ // Do not change to "Awake".
			// Set maxSpeed.
			maxSpeed = transform.parent.GetComponent <Wheel_Control_CS> ().maxSpeed;
		}

		//在Awake()中调用
		//在父对象的所有子对象中查找Track_Scroll_CS脚本的引用
		//根据参考轮的本地坐标位置，确定左右参考轮
		//如果找不到左右参考轮，则输出错误信息并销毁脚本
		void Find_Reference_Wheels ()
		{
			Track_Scroll_CS[] scrollScripts = transform.parent.GetComponentsInChildren <Track_Scroll_CS> ();
			foreach (Track_Scroll_CS scrollScript in scrollScripts) {
				Rigidbody tempRigidbody = scrollScript.referenceWheel.GetComponent <Rigidbody> ();
				if ((tempRigidbody.transform.localPosition.y > 0.0f)) { // Left
					leftReferenceWheel = tempRigidbody;
				} else { // Right
					rightReferenceWheel = tempRigidbody;
				}
			}
			if (leftReferenceWheel == null || rightReferenceWheel == null) {
				Debug.LogError ("Reference Wheels are not assigned in the 'Engine_Sound'.");
				Destroy (this);
			}
		}

		//每帧调用一次
		//如果存在左右参考轮，则计算左右参考轮的角速度和线速度
		//根据左右参考轮的线速度平均值与最大速度的比值，计算当前速率
		//使用Mathf.Lerp()方法根据当前速率插值计算音调和音量，并将其应用于AudioSource
		void Update ()
		{
			if (leftReferenceWheel && rightReferenceWheel) {
				float leftVelocity;
				float rightVelocity;
				leftVelocity = leftReferenceWheel.angularVelocity.magnitude / DOUBLE_PI * leftCircumference;
				rightVelocity = rightReferenceWheel.angularVelocity.magnitude / DOUBLE_PI * rightCircumference;
				float targetRate = (leftVelocity + rightVelocity) / 2.0f / maxSpeed;
				currentRate = Mathf.MoveTowards (currentRate, targetRate, 0.02f);
				thisAudioSource.pitch = Mathf.Lerp (minPitch, maxPitch, currentRate);
				thisAudioSource.volume = Mathf.Lerp (minVolume, maxVolume, currentRate);
			}
		}

		//从"Damage_Control_CS"脚本调用
		//销毁当前游戏对象
		void Destroy ()
		{ // Called from "Damage_Control_CS".
			Destroy (this.gameObject);
		}

		//从"Game_Controller_CS"脚本调用
		//根据传入的isPaused参数启用或禁用脚本
		//如果isPaused为true，将音量设置为0.0
		void Pause (bool isPaused)
		{ // Called from "Game_Controller_CS".
			this.enabled = !isPaused;
			if (isPaused) {
				thisAudioSource.volume = 0.0f;
			}
			
		}

	}

}
