using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//鼠标控制视角
public class CameraMove : MonoBehaviour
{
    //鼠标x轴灵敏度
    public float mouseXSensitivity = 25f;
    //人物
    private Transform player;
    //旋转角度
    float xRotation = 0f;

    private void Start()
    {
        player = transform.parent.transform;
    }


    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseXSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseXSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        //y轴最大旋转角度为正负90;
        xRotation = Mathf.Clamp(xRotation, -45f, 10f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.Rotate(Vector3.up * mouseX);
    }
}
