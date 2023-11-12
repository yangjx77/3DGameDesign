using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//动作管理类基类
public class SSActionManager : MonoBehaviour
{
    //定义一个字典，用于存储正在执行的动作，以动作实例的唯一标识符作为键
    public Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
    //定义一个列表，用于存储等待添加到aactions字典中的动作实例
    private List<SSAction> waitingAdd = new List<SSAction>();
    //定义一个列表，用于存储等待从actions字典中删除动作实例的唯一标识符
    private List<int> waitingDelete = new List<int>(); 
    // Start is called before the first frame update
    protected void Start(){}

    // Update is called once per frame
    protected void Update()
    {
        //首先将列表中等待添加到actions字典中的动作实例添加到actions字典中
        foreach (SSAction ac in waitingAdd) {
            actions[ac.GetInstanceID()] = ac;
        }
        //清空waitingAdd列表
        waitingAdd.Clear();

        //遍历在actions字典中的动作实例
        foreach(KeyValuePair<int, SSAction> kv in actions) {
            SSAction ac = kv.Value;
            //如果动作实例中的destroy为True，将该动作实例添加到等待删除waitingDelete的列表中
            if (ac.destroy) {
                waitingDelete.Add(ac.GetInstanceID());
            } 
            //如果动作实例的enable为True，就调用其Update方法进行更新
            else if (ac.enable) {
                ac.Update();
            }
        }

        //根据waitingDelete列表中的唯一标识符，从actions字典中删除相对应的动作实例
        foreach(int key in waitingDelete) {
            SSAction ac = actions[key];
            actions.Remove(key);
            Destroy(ac);
        }
        //清空waitingDelete列表
        waitingDelete.Clear();
    }

    //定义一个方法，用于运行一个动作
    //接收一个游戏对象，一个动作实例，以及一个动作回调接口
    public void RunAction(GameObject gameObject, SSAction action, IActionCallback manager) {
        //为动作实例设置游戏对象和变换属性
        action.gameObject = gameObject;
        action.transform = gameObject.transform;
        action.callback = manager;
        //将动作实例添加到waitingAdd列表中
        waitingAdd.Add(action);
        //调用动作实例的start方法
        action.Start();
    }

    //定义一个虚函数，用于移动飞碟的行为
    //通过子类重写来实现特定的移动行为
    public virtual void MoveDisk(GameObject disk){}
}
