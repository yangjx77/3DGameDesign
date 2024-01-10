using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//用于将游戏对象移动到目标点
public class PlaceTarget : MonoBehaviour
{
    //public GameObject target：用于获取目标点的游戏对象，在Inspector面板中进行赋值
    //NavMeshAgent mr：NavMesh代理组件的引用
    public GameObject target;  //获取目标点，注意在面板中赋值
    NavMeshAgent mr;   //声明变量
                       // Use this for initialization
    
    //在脚本启动时调用
    //获取自身的NavMeshAgent组件，并将其赋值给mr变量
    void Start()
    {
        //获取到自身的NavMeshAgent组件
        mr = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    //每帧调用一次
    //使用NavMeshAgent的SetDestination()方法将目标点的位置传递给NavMeshAgent组件，以使游戏对象移动到目标点
    void Update()
    {
        //使用属性将目标点的坐标进行传递
        //mr.destination = target.transform.position;
        //使用方法获取目标点坐标，，和前一行代码作用相同
        mr.SetDestination(target.transform.position);
    }
}