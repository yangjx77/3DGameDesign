using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用于控制对象的移动行为
public class Move : MonoBehaviour
{
    //用于标记该对象是否正在移动
    public bool isMoving = false;
    //表示该对象的移动速度
    public float speed = 5;
    //表示移动的对象的目标位置
    public Vector3 destination;
    //表示了移动对象到目标位置的中间位置，主要是为了产生一个动态以及美观的移动
    public Vector3 mid_destination;
    // Update is called once per frame
    void Update()
    {
        //如果对象的移动目标等于对象正在的位置上，那么表示没有移动
        if (transform.localPosition == destination) {
            isMoving = false;
            return;
        }
        //否则，就是角色正在移动
        isMoving = true;
        //如果移动目标的x与y位置与角色正在的位置不同，那么移动该角色到目标的中间位置
        if (transform.localPosition.x != destination.x && transform.localPosition.y != destination.y) {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, mid_destination, speed * Time.deltaTime);
        }
        //否则，表示角色已经到达了中间位置，需要将角色移动到目标位置
        else {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, destination, speed * Time.deltaTime);
        }
    }
}
