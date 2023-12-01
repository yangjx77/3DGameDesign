using UnityEngine;
using UnityEngine.UI;

public class Bow : MonoBehaviour
{
    //导入箭的预制体
    public GameObject arrowPrefab;  
    //箭的Transform组件
    public Transform arrowSpawnPoint;  
    //弓的最大拉动距离
    public float maxPullDistance = 3f; 
    //弓的最大拉动力度
    public float maxPullForce = 100f; 
    //弓的最小拉动时间
    public float minPullTime = 1f; 
    //弓的最大拉动时间
    public float maxPullTime = 5f; 
    //箭的飞行速度
    public float arrowFlightSpeed = 10f; 

    //开始的拉动时间
    private float pullStartTime;
    //拉动的距离 
    private float pullDistance; 
    //弓的动画控制器
    private Animator anim;

    //射击区域
    public ShootingArea shootingArea;
    //箭的数量text
    public Text arrowCountTxt;
    //箭的数量UI
    public GameObject arrowCount;

    //游戏介绍的UI
    public GameObject over;

    void Start()
    {
        Time.timeScale = 1;
        anim = GetComponent<Animator>();
        LockCursor(true);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Cursor.visible) { LockCursor(false); }

        if (Input.GetMouseButtonDown(0) && Cursor.visible == false) { LockCursor(true); }

        if (shootingArea == null) 
        {
            arrowCount.SetActive(false);
            return;
        }
        else
        {
            arrowCount.SetActive(true);
            arrowCountTxt.text = "箭数:" + shootingArea.arrowCount;
        }
        

        if (shootingArea.isArrow && shootingArea.arrowCount > 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                pullStartTime = 0;
                anim.SetTrigger("hold");
                //调用该函数清除场景中的箭
                FindBullet();
            }
            else if (Input.GetMouseButton(0))
            {
                //增加拉动箭头的时间
                pullStartTime += Time.deltaTime;
                //将拉动箭头的时间设置为前面计算得到的时间
                anim.SetFloat("holdTime", pullStartTime);
            }//玩家松开鼠标左键释放箭
            else if (Input.GetMouseButtonUp(0))
            {
                pullDistance = pullStartTime;
                pullStartTime = 0;
                anim.SetTrigger("shoot");
                ShootArrow();
                Invoke("FindShootingArea", 1.5f);
            }
        }
    }

    private void ShootArrow()
    {
        // 实例化箭
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        Rigidbody arrowRigidbody = arrow.GetComponent<Rigidbody>();
        //根据拉动距离给箭设定速度
        arrowRigidbody.velocity = transform.forward * pullDistance * 30f;
        shootingArea.arrowCount -= 1;
        arrowCountTxt.text = "箭数:" + shootingArea.arrowCount;
    }

    public void FindBullet()
    {
        var bullets = GameObject.FindGameObjectsWithTag("Bullet");
        for (int i = 0; i < bullets.Length; i++)
        {
            Destroy(bullets[i]);
        }
    }

    public void FindShootingArea()
    {
        
        var ShootingAreas = GameObject.FindGameObjectsWithTag("ShootingArea");
        var temp = 0;
        for (int i = 0; i < ShootingAreas.Length; i++)
        {
            if (ShootingAreas[i].transform.GetComponent<ShootingArea>().arrowCount > 0)
            {
                temp++;
            }
        }
        if (temp <= 0)
        {
            LockCursor(false);
            over.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void LockCursor(bool a)
    {
        if (a)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
