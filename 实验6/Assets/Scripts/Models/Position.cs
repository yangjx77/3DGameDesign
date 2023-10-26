using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position  //存储所有对象的位置
{
    //游戏对象的固定位置（世界坐标）
    public static Vector3 left_land = new Vector3(-8,-3,0);
    public static Vector3 right_land = new Vector3(8,-3,0);
    public static Vector3 river = new Vector3(0,-4,0);
    public static Vector3 left_boat = new Vector3(-2.3f,-2.3f,-0.4f);
    public static Vector3 right_boat = new Vector3(2.4f, -2.3f, -0.4f);

    //角色相对于（在）岸边的位置(相对坐标)
    public static Vector3[] role_land = new Vector3[] {new Vector3(0.4f,0.77f,0), new Vector3(0.25f,0.77f,0), new Vector3(0.1f,0.77f,0), new Vector3(-0.05f,0.77f,0), new Vector3(-0.2f,0.77f,0), new Vector3(-0.35f,0.77f,0)};
    
    //角色相对于（在）船的位置(相对坐标)
    public static Vector3[] role_boat = new Vector3[] {new Vector3(0.2f,3,0), new Vector3(-0.2f,3,0)};


}
