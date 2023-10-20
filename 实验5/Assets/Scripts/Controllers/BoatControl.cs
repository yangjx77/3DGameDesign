using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatControl : ClickAction
{
    //首先创建一个船的类对象
    Boat boatModel;
    //创建一个用户操作动作的类对象，用于用户进行操作
    IUserAction userAction;

    //构造函数
    public BoatControl() {
        //实现使用userAction变量来调用IuserAction接口的方法，处理船的控制逻辑
        userAction = SSDirector.GetInstance().CurrentSceneController as IUserAction;
    }
    //定义一个函数，用来创建一个船的对象
    public void CreateBoat(Vector3 position) {
        //如果船的对象为空，那么就销毁该船的游戏对象
        if (boatModel != null) {
            Object.DestroyImmediate(boatModel.boat);
        }
        //新建一个船的游戏对象，并且赋给类中定义的对象
        boatModel = new Boat(position);
        //获取Click组件，调用setClickAction方法
        boatModel.boat.GetComponent<Click>().setClickAction(this);
    }

    //返回当前的船的游戏对象
    public Boat GetBoatModel() {
        return boatModel;
    }

    //将角色从岸上移动到船上，接收的对象为需要移动的角色对象，返回移动后的角色的位置坐标
    public Vector3 AddRole(Role roleModel) {
        //首先初始化index=-1，然后判断船上是否有空的位置可以容纳角色对象
        int index = -1;
        if (boatModel.roles[0] == null) index = 0;
        else if (boatModel.roles[1] == null) index = 1;

        //如果船上已经满员了，返回角色的原始的位置
        if (index == -1) return roleModel.role.transform.localPosition;

        //如果没有满员，将角色添加到船上的对应位置
        boatModel.roles[index] = roleModel;
        //同时更新角色是否在船上的标记变量
        roleModel.inBoat = true;
        //将角色挂载到船上，以后的位置变换为相对船而变换
        roleModel.role.transform.parent = boatModel.boat.transform;
        //更新船上的角色（牧师还是魔鬼）数量
        if (roleModel.isPriest) boatModel.priestCount++;
        else boatModel.devilCount++;
        //返回角色的变换后的位置信息
        return Position.role_boat[index];
    }

    //将角色从船上移动到岸上，接收参数为角色对象，表示需要移动的对象
    public void RemoveRole(Role roleModel) {
        //循环遍历船上的角色数组
        for (int i = 0; i < 2; ++i){
            //如果当前遍历的角色与需要移动的角色相同，那么就移动该角色到岸上
            if (boatModel.roles[i] == roleModel) {
                boatModel.roles[i] = null;
                //更新船上的角色（牧师还是魔鬼）数量
                if (roleModel.isPriest) boatModel.priestCount--;
                else boatModel.devilCount--;
                break;
            }
        }
    }

    //重写DealClick函数
    public void DealClick() {
        //如果船上的角色数组不为空，那么调用移动角色的函数
        if (boatModel.roles[0] != null || boatModel.roles[1] != null) {
            userAction.MoveBoat();
        }
    }
}
