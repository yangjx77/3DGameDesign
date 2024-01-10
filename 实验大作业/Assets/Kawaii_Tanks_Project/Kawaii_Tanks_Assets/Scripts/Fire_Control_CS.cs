using UnityEngine;
using System.Collections;
using UnityEngine.UI;

#if UNITY_ANDROID || UNITY_IPHONE
using UnityStandardAssets.CrossPlatformInput;
#endif

// This script must be attached to "Cannon_Base".
//用于控制开火行为
namespace ChobiAssets.KTP
{
	public class Fire_Control_CS : MonoBehaviour
	{

		[Header ("Fire control settings")]
		[Tooltip ("Loading time. (Sec)")] public float reloadTime = 0.1f;
		[Tooltip ("Recoil force with firing.")] public float recoilForce = 100.0f;

		bool isReady = true;
		Transform thisTransform;
		Rigidbody bodyRigidbody;

		ID_Control_CS idScript;
		Barrel_Control_CS[] barrelScripts;
		Fire_Spawn_CS[] fireScripts;

        private float count = 0;


		//在脚本启动时调用
		//获取当前游戏对象的Transform组件
		//获取当前游戏对象下的所有Barrel_Control_CS和Fire_Spawn_CS脚本的引用
		void Awake ()
		{
			thisTransform = this.transform;
			barrelScripts = GetComponentsInChildren <Barrel_Control_CS> ();
			fireScripts = GetComponentsInChildren <Fire_Spawn_CS> ();
		}

		//在每一帧更新时调用
		//如果该游戏对象是玩家操控的坦克（由idScript.isPlayer属性判断），则根据平台类型调用Mobile_Input()或Desktop_Input()来接收输入
		//如果该游戏对象不是玩家操控的坦克，则根据一定时间间隔调用Fire()来进行自动开火
		void Update ()
		{
			if (idScript.isPlayer) {
				#if UNITY_ANDROID || UNITY_IPHONE
				Mobile_Input ();
				#else
				Desktop_Input ();
				#endif
			} else
            {
                count = count + Time.deltaTime;
                if(count > 3.0f)
                {
                    Fire();
                    count = 0;
                }
            }
		}

		//用于在移动平台上接收输入
		//如果按下了名为"GunCam_Press"的按钮，并且当前可以开火（isReady为true），则调用Fire()来进行开火
		#if UNITY_ANDROID || UNITY_IPHONE
		void Mobile_Input ()
		{
			if (CrossPlatformInputManager.GetButtonUp ("GunCam_Press") && isReady) {
				Fire ();
			}
		}
		#else

		//用于在桌面平台上接收输入
		//如果设置了fireButton，并且当前可以开火（isReady为true），则调用Fire()来进行开火
		void Desktop_Input ()
		{
			if (idScript.fireButton && isReady) {
				Fire ();
			}
		}
		#endif

		//开火函数
		//调用barrelScripts和fireScripts的Fire()方法来执行开火操作
		//如果该游戏对象是玩家操控的坦克（由idScript.isPlayer属性判断），则执行后坐力效果，并进入重新装填状态
		//启动Reload协程来控制重新装填时间
		void Fire ()
		{
			// Call barrelScripts and fireScripts to fire.
			for (int i = 0; i < barrelScripts.Length; i++) {
				barrelScripts [i].Fire ();
			}
			for (int i = 0; i < fireScripts.Length; i++) {
				fireScripts [i].StartCoroutine ("Fire");
			}
            if(idScript.isPlayer)
            {
                // Add recoil shock.
                bodyRigidbody.AddForceAtPosition(-thisTransform.forward * recoilForce, thisTransform.position, ForceMode.Impulse);
                isReady = false;
                StartCoroutine("Reload");
            }
			
		}

		//重新装填协程
		//在一定时间（reloadTime）后将isReady设置为true，表示可以进行下一次开火
		IEnumerator Reload ()
		{
			yield return new WaitForSeconds (reloadTime);
			isReady = true;
		}

		//被"Damage_Control_CS"调用
		//销毁当前脚本组件
		void Destroy ()
		{ // Called from "Damage_Control_CS".
			Destroy (this);
		}

		//由"ID_Control_CS"调用
		//获取ID_Control_CS脚本的引用，并缓存其属性值
		//获取关联的刚体组件的引用
		void Get_ID_Script (ID_Control_CS tempScript)
		{
			idScript = tempScript;
			bodyRigidbody = idScript.storedTankProp.bodyRigidbody;
		}

		//由"Game_Controller_CS"调用
		//根据传入的isPaused参数，启用或禁用当前脚本组件，实现暂停或恢复行为控制
		void Pause (bool isPaused)
		{ // Called from "Game_Controller_CS".
			this.enabled = !isPaused;
		}

	}

}
