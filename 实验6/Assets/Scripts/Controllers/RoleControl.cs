using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用于控制对象的动作
public class RoleControl : ClickAction
{
    //首先创建一个需要操作的角色对象
    Role roleModel;
    //创建一个用户操作动作的类对象，用于用户进行操作
    IUserAction userAction;

    //构造函数，实现使用userAction变量来调用IuserAction接口的方法，处理角色的控制逻辑
    public RoleControl() {
        userAction = SSDirector.GetInstance().CurrentSceneController as IUserAction;
    }

    //用于创建一个新的角色对象，接收参数为位置信息，是否为牧师信息，以及角色的id号
    public void CreateRole(Vector3 position, bool isPriest, int id) {
        //如果角色对象不为空，那么将该对象销毁
        if (roleModel != null) {
            Object.DestroyImmediate(roleModel.role);
        }
        //创建一个新的角色对象
        roleModel = new Role(position, isPriest, id);
        //获取Click组件，调用setClickAction方法来处理
        roleModel.role.GetComponent<Click>().setClickAction(this);
    }

    //获取返回的角色对象
    public Role GetRoleModel() {
        return roleModel;
    }

    //重写DealClick函数，调用移动角色的函数
    public void DealClick() {
        userAction.MoveRole(roleModel);
    }
}
