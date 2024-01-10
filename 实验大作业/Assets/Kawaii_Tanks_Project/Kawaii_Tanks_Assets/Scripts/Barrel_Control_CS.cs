using UnityEngine;
using System.Collections;

// This script must be attached to "Barrel_Base".
//用于控制炮管的坐后力效果
namespace ChobiAssets.KTP
{
	
	public class Barrel_Control_CS : MonoBehaviour
	{
		//设置了浮点型变量，用于设置了坐后力制动器地参数
		[ Header ("Recoil Brake settings")]
		[ Tooltip ("Time it takes to push back the barrel. (Sec)")] public float recoilTime = 0.05f;    //炮管坐后力时间
		[ Tooltip ("Time it takes to to return the barrel. (Sec)")] public float returnTime = 0.05f;    //炮管回位的持续时间
		[ Tooltip ("Movable length for the recoil brake. (Meter)")] public float length = 0.3f;         //炮管移动的长度

		Transform thisTransform;           //组件变量
		bool isReady = true;               //表示坐后力效果是否准备就绪
		Vector3 initialPos;                //用于存储炮管的初始本地位置
		const float HALF_PI = Mathf.PI * 0.5f;   //用于后续的计算

		void Awake ()
		{
			//绑定和初始化组件
			thisTransform = this.transform;
			initialPos = thisTransform.localPosition;
		}

		//一个协程方法，用于控制炮管的后坐力效果
		IEnumerator Recoil_Brake ()
		{
			//用于后坐力前冲
			// Move backward.
			//在一定的时间recoilTime内，通过计算当前时间与总时间的比例
			//使用正弦函数计算炮管的位置偏移量，并将其应用到炮管的本地坐标上
			float count = 0.0f;
			while (count < recoilTime) {
				float rate = Mathf.Sin (HALF_PI * (count / recoilTime));
				thisTransform.localPosition = new Vector3 (initialPos.x, initialPos.y, initialPos.z - (rate * length));
				count += Time.deltaTime;
				yield return null;
			}
			// Return to the initial position.
			//用于炮管回位
			//在一定的时间returnTime内，通过计算当前时间与总时间的比例
			//使用正弦函数计算炮管的位置偏移量，并将其应用到炮管的本地坐标上
			count = 0.0f;
			while (count < returnTime) {
				float rate = Mathf.Sin (HALF_PI * (count / returnTime) + HALF_PI);
				thisTransform.localPosition = new Vector3 (initialPos.x, initialPos.y, initialPos.z - (rate * length));
				count += Time.deltaTime;
				yield return null;
			}
			//表示后坐力效果已经完成
			isReady = true;
		}

		//用于触发后坐力效果
		public void Fire ()
		{ // Called from "Fire_Control_CS".
			//首先检查是否已经准备就绪
			if (isReady) {
				isReady = false;
				//启动协程
				StartCoroutine ("Recoil_Brake");
			}
		}
	}

}
