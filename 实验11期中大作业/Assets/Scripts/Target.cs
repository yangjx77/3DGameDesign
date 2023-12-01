using UnityEngine;

public class Target : MonoBehaviour
{
    //得分
    public int score = 1; 
    //是否为运动靶子
    public bool isSportsTarget;
    //靶子的位置点
    private Transform point;
    //靶子的索引
    public int indexTarget;

    private void Start()
    {
        //获取靶子的父对象作为位置点
        point = transform.parent;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // 调用函数计算得分
            CalculateScore();
            collision.transform.GetComponent<Rigidbody>().isKinematic = true;
            collision.transform.position = new Vector3(collision.contacts[0].point.x, collision.contacts[0].point.y, collision.contacts[0].point.z - Random.Range(-0.3f, -0.5f));
            collision.gameObject.transform.parent = point;
        }
    }

    private void CalculateScore()
    {
        //如果靶子的标签为Bullseye，即为靶心
        if (gameObject.tag == "Bullseye")
        {
            // 如果为运动靶子
            if (isSportsTarget)
            {
                // 将得分加3
                Tips.Instance.SetScore(10);
                Tips.Instance.SetText("在" + indexTarget + "号射击位上射中" + indexTarget + "号靶子,加10分");

            }
            else
            {
                //如果不是运动靶子，将得分加2
                Tips.Instance.SetScore(8);
                Tips.Instance.SetText("在" + indexTarget + "号射击位上射中" + indexTarget + "号靶子,加3分");
            }
        }
        // 如果靶子的标签为Circle，即为白色区域
        else if (gameObject.tag == "Circle")
        {
            //如果为运动靶子
            if (isSportsTarget)
            {
                // 将得分加2
                Tips.Instance.SetScore(5);
                Tips.Instance.SetText("在" + indexTarget + "号射击位上射中" + indexTarget + "号靶子,加5分");
            }
            else
            {
                // 如果不是运动靶子，就将得分加1
                Tips.Instance.SetScore(3);
                Tips.Instance.SetText("在" + indexTarget + "号射击位上射中" + indexTarget + "号靶子,加3分");
            }
        }
    }
}

