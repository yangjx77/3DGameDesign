using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Click : MonoBehaviour
{
    //声明变量，用于存储点击事件的处理方法
    ClickAction clickAction;
    //用于接收点击事件的参数，并且将其传给类中定义的clickAction
    public void setClickAction(ClickAction clickAction) {
        this.clickAction = clickAction;
    }
    //unity的一个内置方法，可以将点击事件的处理方式委托给其它方法
    void OnMouseDown() {
        clickAction.DealClick();
    }
}
