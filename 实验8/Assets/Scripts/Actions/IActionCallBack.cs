using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//定义了一个枚举类型，主要有两个状态，分别表示动作事件的开始和完成
public enum SSActionEventType:int {Started, Completed}
//定义一个接口，用于接收动作事件的回调函数
public interface IActionCallback
{
    //回调函数
    //source是触发动作事件的SSAction对象
    //events是事件类型（开始和完成）
    void SSActionEvent(SSAction source,
        SSActionEventType events = SSActionEventType.Completed,
        int intParam = 0,
        string strParam = null,
        Object objectParam = null);
}