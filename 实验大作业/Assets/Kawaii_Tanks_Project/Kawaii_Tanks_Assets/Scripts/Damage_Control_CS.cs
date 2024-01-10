using UnityEngine;
using System.Collections;
using UnityEngine.UI;

#if UNITY_ANDROID || UNITY_IPHONE
using UnityStandardAssets.CrossPlatformInput;
#endif


//用于控制坦克的受伤和销毁的逻辑
namespace ChobiAssets.KTP
{
	
	public class Damage_Control_CS : MonoBehaviour
	{
		[Header ("Damage settings")]
		[Tooltip ("Durability of this tank.")] public float durability = 300.0f;
		[Tooltip ("Prefab used for destroyed effects.")] public GameObject destroyedPrefab;
		[Tooltip ("Prefab of Damage Text.")] public GameObject textPrefab;
		[Tooltip ("Name of the Canvas used for Damage Text.")] public string canvasName = "Canvas_Texts";

		Transform bodyTransform;
		Damage_Display_CS displayScript;
		float initialDurability;

		ID_Control_CS idScript;

		//在启动脚本时调用
		//记录初始耐久度(initialDurability)
		void Start ()
		{ // Do not change to "Awake()".
			initialDurability = durability;
		}

		//根据设置的参数实例化伤害文本(Damage Text)的预制体，并将其设置到Canvas中进行显示
		//获取Damage_Display_CS脚本的引用，用于更新伤害文本的显示
		//根据设置的Canvas名称(canvasName)找到对应的Canvas对象，并将伤害文本设置为其子对象
		//如果找不到对应的Canvas对象，则打印警告信息
		void Set_DamageText ()
		{
			if (textPrefab == null || string.IsNullOrEmpty (canvasName) || durability == Mathf.Infinity) {
				return;
			}
			// Instantiate Damage Text, and set it to the Canvas.
			GameObject textObject = Instantiate (textPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			displayScript = textObject.GetComponent <Damage_Display_CS> ();
			displayScript.targetTransform = bodyTransform;
			GameObject canvasObject = GameObject.Find (canvasName);
			if (canvasObject) {
				displayScript.transform.SetParent (canvasObject.transform);
				displayScript.transform.localScale = Vector3.one;
			} else {
				Debug.LogWarning ("Canvas for Damage Text cannot be found.");
			}
		}

		//在每一帧更新时调用
		//如果是玩家控制的坦克，根据玩家的输入决定是否启动销毁逻辑
		//如果是玩家控制的坦克，调用displayScript.Get_Damage()更新伤害文本的显示
		void Update ()
		{
            // 游戏玩家一直显示血条，AI坦克受到攻击后显示血条
            if(idScript.isPlayer)
            {
                displayScript.Get_Damage(durability, initialDurability);
            }
            // Destruct
            if (idScript.isPlayer) {
				#if UNITY_ANDROID || UNITY_IPHONE
				if (CrossPlatformInputManager.GetButtonDown ("Destruct")) {
				#else
				if (Input.GetKeyDown (KeyCode.Return)) {
				#endif
					Start_Destroying ();
				}
			}
		}

		//从子弹脚本(Bullet_Nav_CS)调用
		//根据传入的伤害值(damageValue)减少坦克的耐久度(durability)
		//如果坦克的耐久度仍然大于零，则调用displayScript.Get_Damage()更新伤害文本的显示
		//如果坦克的耐久度小于等于零，则开始销毁坦克
		public void Get_Damage (float damageValue)
		{ // Called from "Bullet_Nav_CS".
			durability -= damageValue;
			if (durability > 0.0f) { // Still alive.
				// Display Text
				if (displayScript) {
					displayScript.Get_Damage (durability, initialDurability);
				}
			} else { // Dead
				Start_Destroying ();
			}
		}

		//开始销毁坦克的逻辑
		//向坦克的所有部件发送销毁消息("Destroy")
		//如果设置了destroyedPrefab，则在坦克位置实例化destroyedPrefab
		//移除伤害文本(displayScript)
		//销毁当前脚本
		void Start_Destroying ()
		{
            if(idScript.isPlayer == false)
            {
                this.gameObject.SetActive(false);
            }
			// Send message to all the parts.
			BroadcastMessage ("Destroy", SendMessageOptions.DontRequireReceiver);
			// Create destroyedPrefab.
			if (destroyedPrefab) {
				GameObject tempObject = Instantiate (destroyedPrefab, bodyTransform.position, Quaternion.identity) as GameObject;
				tempObject.transform.parent = bodyTransform;
			}
			// Remove the Damage text.
			if (displayScript) {
				Destroy (displayScript.gameObject);
			}
			// Destroy this script.
			Destroy (this);
		}

		//从ID_Control_CS脚本中获取对应的组件引用
        //根据获取的组件引用设置坦克的伤害文本
		void Get_ID_Script (ID_Control_CS tempScript)
		{
			idScript = tempScript;
			bodyTransform = idScript.storedTankProp.bodyTransform;
			Set_DamageText ();
		}

		//从"Game_Controller_CS"脚本调用
        //根据传入的参数决定是否暂停当前脚本的更新
		void Pause (bool isPaused)
		{ // Called from "Game_Controller_CS".
			this.enabled = !isPaused;
		}

	}

}
