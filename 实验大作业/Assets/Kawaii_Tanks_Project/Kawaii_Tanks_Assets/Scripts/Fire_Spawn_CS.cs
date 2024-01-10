using UnityEngine;
using System.Collections;

// This script must be attached to "Fire_Point".
//用于生成子弹和火焰效果
namespace ChobiAssets.KTP
{
	
	public class Fire_Spawn_CS : MonoBehaviour
	{

		[ Header ("Firing settings")]
		[ Tooltip ("Prefab of muzzle fire.")] public GameObject firePrefab;
		[ Tooltip ("Prefab of bullet.")] public GameObject bulletPrefab;
		[ Tooltip ("Attack force of the bullet.")] public float attackForce = 100.0f;
		[ Tooltip ("Speed of the bullet. (Meter per Second)")] public float bulletVelocity = 250.0f;
		[ Tooltip ("Offset distance for spawning the bullet. (Meter)")] public float spawnOffset = 1.0f;

		Transform thisTransform;

		//在脚本启动时调用
		//获取当前游戏对象的Transform组件
		void Awake ()
		{
			thisTransform = this.transform;
		}

		//开火协程
		//实例化火焰效果（如果有firePrefab）并放置在当前游戏对象的位置和旋转下
		//实例化子弹（如果有bulletPrefab）并放置在当前游戏对象前方一定偏移距离的位置
		//获取子弹对象上的Bullet_Nav_CS脚本引用，并设置attackForce属性
		//计算子弹的初始速度tempVelocity，以当前游戏对象的前方方向和设定的子弹速度（bulletVelocity）为基础
		//在下一个FixedUpdate周期中设置子弹刚体的速度为tempVelocity，实现子弹的发射
		public IEnumerator Fire ()
		{
			// Muzzle Fire
			if (firePrefab) {
				GameObject fireObject = Instantiate (firePrefab, thisTransform.position, thisTransform.rotation) as GameObject;
				fireObject.transform.parent = thisTransform;
			}
			// Shot Bullet
			if (bulletPrefab) {
				GameObject bulletObject = Instantiate (bulletPrefab, thisTransform.position + thisTransform.forward * spawnOffset, thisTransform.rotation) as GameObject;
				bulletObject.GetComponent <Bullet_Nav_CS> ().attackForce = attackForce;
				Vector3 tempVelocity = thisTransform.forward * bulletVelocity;
				// Shoot
				yield return new WaitForFixedUpdate ();
				bulletObject.GetComponent <Rigidbody> ().velocity = tempVelocity;
			}
		}

	}

}
