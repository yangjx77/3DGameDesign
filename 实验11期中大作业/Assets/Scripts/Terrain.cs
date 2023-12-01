using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // 如果碰撞的物体标签为"Bullet"
        if (collision.gameObject.CompareTag("Bullet"))
        {
            //将其设置为静态
            collision.transform.GetComponent<Rigidbody>().isKinematic = true;
            collision.transform.position = new Vector3(collision.contacts[0].point.x, collision.contacts[0].point.y, collision.contacts[0].point.z - Random.Range(-0.5f, -0.8f));
        }
    }
}
