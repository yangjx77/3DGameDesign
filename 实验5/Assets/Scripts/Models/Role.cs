using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role 
{   //创建一个角色的游戏对象
    public GameObject role;
    //创建一个变量判断该角色是否为牧师
    public bool isPriest;
    //创建一个变量判断角色是否在船上
    public bool inBoat;
    //创建一个变量判断角色是否在岸的右边
    public bool onRight;
    //创建一个变量来标记该角色是属于第几个对象
    public int id;
    
    //Role的构造函数，接受参数为position、isPriest以及id
    public Role (Vector3 position, bool isPriest, int id) {
        //绑定参数
        this.isPriest = isPriest;
        this.id = id;
        //初始化各个变量
        onRight = false;
        inBoat = false;
        //从资源中加载角色的预制体，并且使用Instantiate方法创建该预制体的游戏对象
        //判断传进来的参数isPriest是否为True，如果是，那么加载牧师的预制体
        //如果不是，就加载魔鬼的预制体
        role = GameObject.Instantiate(Resources.Load("prefab/" + (isPriest ? "priest" : "devil"), typeof(GameObject))) as GameObject;
        //重命名该游戏角色
        role.name = "role" + id; 
        //设置角色在各个方向轴上的缩放比例大小
        role.transform.localScale = new Vector3(1,1.2f,1);
        //将角色的position设置为构造函数传进来的position
        role.transform.position = position;

        //接收组件盒子碰撞以及点击事件
        role.AddComponent<Click>();
        role.AddComponent<BoxCollider>();
    }
}
