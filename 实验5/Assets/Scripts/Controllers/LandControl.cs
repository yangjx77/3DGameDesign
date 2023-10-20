using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandControl
{
    //首先创建一个陆地的对象
    Land landModel;
    //用于创建陆地对象，接收参数为position
    public void CreateLand(Vector3 position) {
        //如果陆地对象为空，那么创建一个新的陆地对象
        if (landModel == null) {
            landModel = new Land(position);
        }
    }

    //用于获取陆地的对象
    public Land GetLand() {
        return landModel;
    }

    //将角色添加到岸上，接收参数为一个角色对象，然后返回角色在岸上的相对坐标
    public Vector3 AddRole2Land(Role roleModel) {
        //将角色挂载到陆地对象上，那么以后的操作都是相对于陆地了
        roleModel.role.transform.parent = landModel.land.transform;
        //更新角色对象中的是否在船上的变量为false
        roleModel.inBoat = false;
        //更新陆地上的角色（牧师还是魔鬼）数量
        if (roleModel.isPriest) landModel.priestCount++;
        else landModel.devilCount++;
        //返回角色移动后的位置信息
        return Position.role_land[roleModel.id];
    }


    //将角色从岸上移除
    public void RemoveRole(Role roleModel) {
        //更新陆地对象中，角色（牧师还是魔鬼）的数量
        if (roleModel.isPriest) landModel.priestCount--;
        else landModel.devilCount--;
    }
}
