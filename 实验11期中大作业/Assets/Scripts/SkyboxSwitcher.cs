using UnityEngine;

public class SkyboxSwitcher : MonoBehaviour
{
    //第一个天空盒
    public Material skybox1; 
    //第二个天空盒
    public Material skybox2; 

    //当前激活的天空盒是否为天空盒1
    private bool isSkybox1Active = true; 

    private void Update()
    {
        //如果按下C键，切换天空盒
        if (Input.GetMouseButtonDown(1))
        {
            SwitchSkybox();
        }
    }

    private void SwitchSkybox()
    {
        //切换天空盒的状态
        isSkybox1Active = !isSkybox1Active;

        if (isSkybox1Active)
        {
            //设置渲染设置中的天空盒材质为第一个天空盒材质
            RenderSettings.skybox = skybox1;
        }
        else
        {
            //设置渲染设置中的天空盒材质为第二个天空盒材质
            RenderSettings.skybox = skybox2;
        }
    }
}
