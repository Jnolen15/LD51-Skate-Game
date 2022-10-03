using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Bomb : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timer;
    [SerializeField] TextMeshProUGUI bombScore;
    [SerializeField] Image bar;

    [SerializeField] GameObject player;
    [SerializeField] PlayerControler pc;
    [SerializeField] Score score;

    [SerializeField] private int curBombScore;
    [SerializeField] private float bombTimer;
    [SerializeField] private int prevScore;

    [SerializeField] private float test;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerControler>();
        score = player.GetComponent<Score>();
        
        curBombScore = 1000;
        bombTimer = 10;
    }

    void Update()
    {
        // Timer and Rad meter display Update
        timer.text = bombTimer.ToString("F2");
        bombScore.text = ("Quota: " + curBombScore);
        var temp = (float)score.score;
        bar.fillAmount = (temp - prevScore) / (curBombScore - prevScore);

        // Bomb timer updating
        if(score.score >= curBombScore)
            ResetTimer();
        else if (bombTimer > 0) bombTimer -= Time.deltaTime;
        else
        {
            if (score.score < curBombScore)
                Explode();
            else 
                ResetTimer();
        }
    }

    private void ResetTimer()
    {
        bombTimer = 10;
        prevScore = score.score;
        curBombScore = (score.score + (int)(score.score * 0.4f));
    }

    private void Explode()
    {
        pc.Explode();
    }
}
