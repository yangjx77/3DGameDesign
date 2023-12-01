using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMove : MonoBehaviour
{
    //靶子的移动速度
    public float speed = 5f; 
    //靶子的移动距离
    public float distance = 10f; 

    //靶子的起始位置
    private Vector3 startPosition;
    //靶子的移动方向
    private float direction = 1f;

    void Start()
    {
        //记录起始位置
        startPosition = transform.position;
    }

    void Update()
    {
        //计算下一帧的位置
        Vector3 nextPosition = transform.position + new Vector3(speed * direction * Time.deltaTime, 0f, 0f);

        // 判断是否超出移动范围，超出则改变移动方向
        if (Vector3.Distance(startPosition, nextPosition) > distance)
        {
            direction *= -1f;
        }

        // 更新位置
        transform.position = nextPosition;
    }
}
