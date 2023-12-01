using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    //资源加载
    public void Load(int level)
    {
        SceneManager.LoadScene(level);
    }
}
