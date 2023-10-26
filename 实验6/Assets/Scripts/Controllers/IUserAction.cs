using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//创建一个接口，定义用户的一些基本操作方式
public interface IUserAction {
    //用于移动船
    void MoveBoat();
    //用于移动角色，将角色从岸上移动到船上
    //将角色从船上移动到岸上
    void MoveRole(Role roleModel);
}

