using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat
{   
    //创建一个船对象
    public GameObject boat;
    //创建一个数组存储在船上的对象
    public Role[] roles;
    //判断该船是否在岸的右边
    public bool isRight;
    //创建变量分别存储船上的牧师和魔鬼的数量
    public int priestCount, devilCount;

    //Boat的构造函数
    public Boat(Vector3 position) {
        //从资源中加载船的预制体，并且使用Instantiate方法创建该预制体的游戏对象
        boat = GameObject.Instantiate(Resources.Load("prefab/boat", typeof(GameObject))) as GameObject;
        //将该游戏对象命名为“boat”
        boat.name = "boat";
        //接受一个position的输入，将我们创建的船的对象的position设置为接收的position
        boat.transform.position = position;
        //设置船对象在各个方向轴上的缩放比例
        boat.transform.localScale = new Vector3(2.8f,0.4f,2);

        //在船上创建一个拥有两个位置的数组
        roles = new Role[2];
        //初始化为船刚开始在岸的左边
        isRight = false;
        //初始化船上的牧师与魔鬼数量为0
        priestCount = devilCount = 0;

        //接受组件盒子碰撞以及点击事件
        boat.AddComponent<BoxCollider>();
        boat.AddComponent<Click>();
        
    }
}
