using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Land 
{
    //创建一个陆地的游戏对象
    public GameObject land;
    //创建变量用于存储在陆地上的牧师与魔鬼的数量
    public int priestCount, devilCount;
    //land的构造函数
    public Land (Vector3 position){
        //从资源中加载陆地的预制体，并且使用Instantiate方法创建该预制体的游戏对象
        land = GameObject.Instantiate(Resources.Load("prefab/land", typeof(GameObject))) as GameObject;
        //设置陆地的游戏对象在各个方向轴上的缩放比例大小
        land.transform.localScale = new Vector3(8,4.8f,2);
        //接受构造函数传进来的position
        //将陆地对象的position设置为传进来的position
        land.transform.position = position;
        //初始化陆地上牧师和魔鬼的数量为0
        priestCount = devilCount = 0;
    }
}
