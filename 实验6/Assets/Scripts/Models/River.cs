using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River
{
    //创建一个河流的游戏对象
    public GameObject river;
    //河流的构造函数，接受传进来的position
    public River(Vector3 position) {
        //从资源中加载河流的预制体，并且使用Instantiate方法创建该预制体的游戏对象
        river = GameObject.Instantiate(Resources.Load("prefab/river", typeof(GameObject))) as GameObject;
        //命名河流游戏对象为river
        river.name = "river";  
        //设置河流在各个方向轴上的缩放比例
        river.transform.localScale = new Vector3(8,2.5f,2);
        //将河流的position设置为构造函数传进来的position
        river.transform.position = position;

    }
}
