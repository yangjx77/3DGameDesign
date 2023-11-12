using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//自定义异常类
public class MyException : System.Exception
{
    //无参构造函数
    public MyException() {}
    //带异常信息的有参构造函数
    public MyException(string message) : base(message) {}
}
//用于存储飞碟的属性信息
public class DiskAttributes : MonoBehaviour
{
    //public GameObject gameobj;
    //分数
    public int score;
    //飞碟的水平方向上的速度
    public float speedX;
    //飞碟的垂直方向上的速度
    public float speedY;
}

//飞碟生成器
//工厂模式
//用于创建和管理飞碟
public class DiskFactory : MonoBehaviour
{
    //用于存储已使用的飞碟游戏对象的列表
    List<GameObject> used;
    //用于存储非使用的飞碟游戏对象的列表
    List<GameObject> free;
    //用于生成随机数
    System.Random rand;

    // Start is called before the first frame update
    void Start()
    {
        //进行了初始化操作
        //将used和free实例化
        //并且创建了一个rand的实例
        used = new List<GameObject>();
        free = new List<GameObject>();
        rand = new System.Random();
    }

    // Update is called once per frame
    void Update(){}

    //用于获取一个飞碟游戏对象
    public GameObject GetDisk(int round) {
        GameObject disk;
        //首先检查未使用的飞碟列表free是否为空
        //如果不为空，就从列表中获取第一个飞碟
        //并且将这个飞碟从free列表中移除
        if (free.Count != 0) {
            disk = free[0];
            free.Remove(disk);
        }
        else {
            //如果free列表为空，就从资源列表中获取一个飞碟预制体
            disk = GameObject.Instantiate(Resources.Load("Prefabs/disk", typeof(GameObject))) as GameObject;
            //添加飞碟属性组件
            disk.AddComponent<DiskAttributes>();
            //添加刚体组件
            disk.AddComponent<Rigidbody>();
        }
        
        //根据不同round设置diskAttributes的值

        //为飞碟的角度设置一个随意的欧拉角
        //其中X轴的旋转角度在-20到-40之间
        disk.transform.localEulerAngles = new Vector3(-rand.Next(20,40),0,0);

        //获取飞碟对象上的飞碟属性组件
        DiskAttributes attri = disk.GetComponent<DiskAttributes>();
        //设置该飞碟属性组件的分数属性为1到3之间
        attri.score = rand.Next(1,4);
        //由分数来决定速度、颜色、大小
        attri.speedX = (rand.Next(1,5) + attri.score + round) * 0.2f;
        attri.speedY = (rand.Next(1,5) + attri.score + round) * 0.2f;
        
        //如果飞碟的分数为3，就将该飞碟的颜色设置为红色，并且稍微缩小一定的比例
        if (attri.score == 3) {
            disk.GetComponent<Renderer>().material.color = Color.red;
            disk.transform.localScale += new Vector3(-0.5f,0,-0.5f);
        }
        //如果飞碟的分数为2，就将该飞碟的颜色设置为绿色，并且稍微缩小一定的比例
        else if (attri.score == 2) {
            disk.GetComponent<Renderer>().material.color = Color.green;
            disk.transform.localScale += new Vector3(-0.2f,0,-0.2f);
        }
        //如果飞碟的分数为1，就将该飞碟的颜色设置为蓝色
        else if (attri.score == 1) {
            disk.GetComponent<Renderer>().material.color = Color.blue;
        }
        
        //飞碟可从四个方向飞入（左上、左下、右上、右下）
        //随机生成一个随机数，用于确定飞碟从哪一个方向飞入
        int direction = rand.Next(1,5);
        //根据前面生成的随机数确定飞碟的飞入方向，根据不同的方向
        //使用disk.transform.Translate将飞碟的初始位置设置在屏幕的不同边缘
        //同时，根据飞碟的不同方向对飞碟的速度进行适当的调整
        //使得飞碟在X轴和Y轴上反向移动
        if (direction == 1) {
            disk.transform.Translate(Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight * 1.5f, 8)));
            attri.speedY *= -1;
        }
        else if (direction == 2) {
            disk.transform.Translate(Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight * 0f, 8)));
            
        }
        else if (direction == 3) {
            disk.transform.Translate(Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight * 1.5f, 8)));
            attri.speedX *= -1;
            attri.speedY *= -1;
        }
        else if (direction == 4) {
            disk.transform.Translate(Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight * 0f, 8)));
            attri.speedX *= -1;
        }
        //将生成的飞碟添加到已使用的飞碟列表中
        used.Add(disk);
        //将飞碟设置为激活状态
        disk.SetActive(true);
        //输出日志信息
        Debug.Log("generate disk");
        //返回该飞碟实例
        return disk;
    }

    //用于释放一个飞碟游戏对象
    public void FreeDisk(GameObject disk) {
        //将飞碟的激活状态设置为False
        disk.SetActive(false);
        //将位置和大小恢复到预制，这点很重要！
        disk.transform.position = new Vector3(0, 0,0);
        disk.transform.localScale = new Vector3(2f,0.1f,2f);
        //检查是否在已使用的飞碟列表used中包含该飞碟对象
        if (!used.Contains(disk)) {
            //如果不包含，就抛出自定义异常
            throw new MyException("Try to remove a item from a list which doesn't contain it.");
        }
        //输出日志信息
        Debug.Log("free disk");
        //将飞碟从已使用的飞碟列表中移除
        used.Remove(disk);
        //将该飞碟对象添加到free对象列表中
        free.Add(disk);
    }
}
