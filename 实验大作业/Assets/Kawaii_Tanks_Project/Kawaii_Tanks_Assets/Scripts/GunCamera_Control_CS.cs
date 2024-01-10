using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//用于控制炮塔摄像机
namespace ChobiAssets.KTP
{
	
	public class GunCamera_Control_CS : MonoBehaviour
	{

		[ Header ("Gun Camera settings")]
		[ Tooltip ("Main Camera of this tank.")] public Camera mainCamera;
		[ Tooltip ("Name of Image for Reticle.")] public string reticleName = "Reticle";

		Camera thisCamera;
		Image reticleImage;

		//在脚本启动时调用
		//设置当前游戏对象的标签为"MainCamera"
		//获取当前游戏对象上的Camera组件，并将其禁用
		//检查是否在"Gun_Camera"脚本的Inspector面板中指定了主摄像机（mainCamera），如果没有指定则输出错误信息，并销毁这个脚本
		//查找名为"Reticle"的游戏对象，并获取其上的Image组件作为瞄准镜图像（reticleImage）
		void Awake ()
		{
			this.tag = "MainCamera";
			thisCamera = GetComponent <Camera> ();
			thisCamera.enabled = false;
			if (mainCamera == null) {
				Debug.LogError ("'Main Camera is not assigned in the Gun_Camera.");
				Destroy (this);
			}
			Find_Image ();
		}

		//查找瞄准镜图像
		//如果指定了瞄准镜图像的名称（reticleName），则在场景中查找该游戏对象，并获取其上的Image组件作为瞄准镜图像（reticleImage）
		//如果未找到瞄准镜图像，则输出警告信息
		void Find_Image ()
		{
			// Find Reticle Image.
			if (string.IsNullOrEmpty (reticleName) == false) {
				GameObject reticleObject = GameObject.Find (reticleName);
				if (reticleObject) {
					reticleImage = reticleObject.GetComponent <Image> ();
				}
			}
			if (reticleImage == null) {
				Debug.LogWarning (reticleName + " (Image for Reticle) cannot be found in the scene.");
			}
		}

		//从"Turret_Control_CS"脚本中调用，用于开启炮塔摄像机
		//禁用主摄像机，启用炮塔摄像机
		//如果存在瞄准镜图像（reticleImage），则启用该图像
		public void GunCam_On ()
		{ // Called from "Turret_Control_CS".
			mainCamera.enabled = false;
			thisCamera.enabled = true;
			if (reticleImage) {
				reticleImage.enabled = true;
			}
		}

		//从"Turret_Control_CS"脚本中调用，用于关闭炮塔摄像机
		//禁用炮塔摄像机，启用主摄像机
		//如果存在瞄准镜图像（reticleImage），则禁用该图像
		public void GunCam_Off ()
		{ // Called from "Turret_Control_CS".
			thisCamera.enabled = false;
			mainCamera.enabled = true;
			if (reticleImage) {
				reticleImage.enabled = false;
			}
		}

		//从"ID_Control_CS"脚本中调用，用于获取ID_Control_CS脚本的引用
		//将当前脚本（GunCamera_Control_CS）的引用（gunCamScript）赋值给ID_Control_CS脚本的gunCamScript变量
		void Get_ID_Script (ID_Control_CS tempScript)
		{
			tempScript.gunCamScript = this;
		}

		//从"ID_Control_CS"脚本中调用，用于切换玩家
		//禁用炮塔摄像机
		//如果存在瞄准镜图像（reticleImage），则禁用该图像
		public void Switch_Player (bool isPlayer)
		{
			thisCamera.enabled = false;
			if (reticleImage) {
				reticleImage.enabled = false;
			}
		}
	}

}
