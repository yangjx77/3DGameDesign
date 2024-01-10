using UnityEngine;
using System.Collections;

// This script must be attached to all the Driving Wheels.
//用于控制车轮的旋转
namespace ChobiAssets.KTP
{
	
	public class Wheel_Rotate_CS : MonoBehaviour
	{

		bool isLeft;                          //用于表示车轮是否位于左侧
		Rigidbody thisRigidbody;              //用于存储车轮的刚体组件
		float maxAngVelocity;                 //最大角速度，根据车辆的最大速度和车轮半径计算得出
		Wheel_Control_CS controlScript;       //用于存储 Wheel_Control_CS 脚本的引用，该脚本控制车辆的行驶 
		Transform thisTransform;              //当前车轮的 Transform 组件
		Transform parentTransform;            //当前车轮的父物体的 Transform 组件
		Vector3 angles;                       //存储车轮的初始旋转角度


		//将车轮的层级设置为第9层，用于车轮
		//获取车轮的刚体组件
		//根据车轮在局部坐标系的位置判断车轮是否在左侧
		//获取车轮的初始旋转角度
		//查找并获取父物体的 Wheel_Control_CS 脚本
		//根据车轮的半径计算最大角速度
		void Awake ()
		{
			this.gameObject.layer = 9; // Layer9 >> for wheels.
			thisRigidbody = GetComponent <Rigidbody> ();
			// Set direction.
			if (transform.localPosition.y > 0.0f) {
				isLeft = true;
			} else {
				isLeft = false;
			}
			// Get initial rotation.
			thisTransform = transform;
			parentTransform = thisTransform.parent;
			angles = thisTransform.localEulerAngles;
			// Find controlScript.
			controlScript = parentTransform.parent.GetComponent <Wheel_Control_CS> ();
			// Set maxAngVelocity.
			float radius = GetComponent <SphereCollider> ().radius;
			maxAngVelocity = Mathf.Deg2Rad * ((controlScript.maxSpeed / (2.0f * Mathf.PI * radius)) * 360.0f);
		}

		/* for reducing Calls.
		public void FixedUpdate_Me ()
		*/
		//根据车轮是左侧还是右侧，获取对应的转速比率
		//根据转速比率和控制脚本中的轮胎扭矩，给车轮施加扭矩
		//设置车轮的最大角速度，根据转速比率和之前计算的最大角速度
		//通过稳定角度来保持车轮的角度
		void FixedUpdate ()
		{
			float rate;
			if (isLeft) {
				rate = controlScript.leftRate;
			} else {
				rate = controlScript.rightRate;
			}

			thisRigidbody.AddRelativeTorque (0.0f, Mathf.Sign (rate) * controlScript.wheelTorque, 0.0f);
			thisRigidbody.maxAngularVelocity = Mathf.Abs (maxAngVelocity * rate);
			// Stabilize angle.
			angles.y = thisTransform.localEulerAngles.y;
			thisRigidbody.rotation = parentTransform.rotation * Quaternion.Euler (angles);
		}

		//当游戏对象被破坏时调用，设置车轮的角阻尼为无穷大，并销毁脚本自身
		void Destroy ()
		{ // Called from "Damage_Control_CS".
			thisRigidbody.angularDrag = Mathf.Infinity;
			Destroy (this);
		}

		//当游戏暂停时调用，根据传入的布尔值参数来启用或禁用脚本的更新
		void Pause (bool isPaused)
		{ // Called from "Game_Controller_CS".
			this.enabled = !isPaused;
		}

	}

}
