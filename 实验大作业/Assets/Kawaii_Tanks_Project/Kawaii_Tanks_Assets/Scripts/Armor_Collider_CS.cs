using UnityEngine;
using System.Collections;

//用于管理装甲的碰撞器的属性
namespace ChobiAssets.KTP
{
	
	public class Armor_Collider_CS : MonoBehaviour
	{
		[ Header ("Armor settings")]
		//用于设置装甲的伤害倍数
		[ Tooltip ("Multiplier for the damage.")] public float damageMultiplier = 1.0f;

		void Awake ()
		{
			// Make it a trigger and invisible.
			//获取当前脚本附加的游戏对象上的碰撞器组件，然后设置其为一个触发器
			GetComponent < Collider > ().isTrigger = true;
			//获取当前脚本附加的游戏对象上的网格渲染器组件，并将其设置为在游戏中不可见
			GetComponent < MeshRenderer > ().enabled = false;
		}

	}

}
