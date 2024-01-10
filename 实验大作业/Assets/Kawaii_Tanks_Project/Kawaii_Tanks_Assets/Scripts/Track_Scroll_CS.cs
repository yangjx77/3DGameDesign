using UnityEngine;
using System.Collections;

// This script must be attached to Tracks.
//用于实现履带滚动
namespace ChobiAssets.KTP
{
	
	public class Track_Scroll_CS : MonoBehaviour
	{
		[ Header ("Scroll Animation settings")]
		[ Tooltip ("Reference wheel.")] public Transform referenceWheel;      //参考轮的Transform组件，用作滚动动画的参考点
		[ Tooltip ("Scroll Rate for X axis.")] public float scrollRate = 0.0005f;      //X轴的滚动速率
		//在着色器中使用的纹理名称
 		[ Tooltip ("Texture Name in the shader.")] public string textureName = "_MainTex"; // "_DetailAlbedoMap" (for Standard shader).

		Material thisMaterial;         //履带的材质
		float previousAng;             //前一帧参考轮的旋转角度
 		float offsetX;                 //纹理的X轴偏移量

		//在脚本启动时调用
		//获取渲染器组件的材质引用	
		//检查是否已分配参考轮，如果没有，则输出错误信息并销毁脚本
		void Awake ()
		{
			thisMaterial = GetComponent <Renderer> ().material;
			if (referenceWheel == null) {
				Debug.LogError ("Reference Wheel is not assigned in " + this.name);
				Destroy (this);
			}
		}

		//每帧调用一次
		//获取当前帧参考轮的旋转角度
		//计算当前帧与上一帧参考轮旋转角度的差值
		//根据滚动速率和角度差值计算X轴偏移量
		//将X轴偏移量应用于材质的纹理偏移量，实现履带纹理的滚动效果
		//更新前一帧参考轮的旋转角度为当前角度
		void Update ()
		{
			float currentAng = referenceWheel.localEulerAngles.y;
			float deltaAng = Mathf.DeltaAngle (currentAng, previousAng);
			offsetX += scrollRate * deltaAng;
			thisMaterial.SetTextureOffset (textureName, new Vector2 (offsetX, 0.0f));
			previousAng = currentAng;
		}

		//从"Game_Controller_CS"脚本调用
		//根据传入的isPaused参数启用或禁用脚本
		void Pause (bool isPaused)
		{ // Called from "Game_Controller_CS".
			this.enabled = !isPaused;
		}
	}

}
