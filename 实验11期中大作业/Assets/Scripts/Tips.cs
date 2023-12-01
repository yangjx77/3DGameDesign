using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tips : MonoBehaviour
{
    public static Tips Instance;
    public GameObject tips;
    public Text tipsText;

    private int score;
    public Text scoreText;

    private void Awake()
    {
        Instance = this;
    }

    public void SetText(string str)
    {
        tipsText.text = str;
        tips.SetActive(true);
    }

    public void SetScore(int score)
    {
        this.score += score;
        scoreText.text = "分数:" + this.score;
    }
}
