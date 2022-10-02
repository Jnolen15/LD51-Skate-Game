using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public PlayerControler pc;
    public UIScript uiScript;
    public TextMeshProUGUI moveText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI TotalScoreText;
    public int score;

    [SerializeField] private bool grind;
    [SerializeField] private bool grounded;
    [SerializeField] private int combo;
    [SerializeField] private int comboScore;
    [SerializeField] private List<string> combolist = new List<string>();
    [SerializeField] private List<string> movelist = new List<string>();

    // Bomb timer will increase top speed

    private void Update()
    {
        // Update grind and grounded bools
        if (!grounded && !pc.onRail && pc.grounded)
        {
            grounded = true;
        } else if (pc.onRail || !pc.grounded)
        {
            grounded = false;
        }

        if (!grind && pc.onRail)
        {
            grind = true;
        }
        else if(grind && grounded)
        {
            grind = false;
        }

        if(grounded && combo!= 0)
        {
            EndCombo();
            combo = 0;
        }

        // Update total score
        TotalScoreText.text = ("Score: " + score);
    }

    public void AddScore(int points, string moveName)
    {
        moveText.text = moveName;
        movelist.Add(moveName);

        // If the move is new to this combo add it to the list
        if (!combolist.Contains(moveName) && points > 0)
        {
            combolist.Add(moveName);
        }

        if (points > 0)
        {
            score += points;
            scoreText.text = ("+" + points);
            comboScore += points;
            combo++;
            if (combo > 1)
                comboText.text = (combo + "X Combo!");
        } else
        {
            score += points;
            scoreText.text = (points.ToString());
            EndCombo();
        }
    }

    private void EndCombo()
    {
        if(combo != 0)
        {
            uiScript.ClearComboList();

            // Combo bonus
            var bonus = combo * 0.1f;
            var comboPoints = (int)(comboScore * bonus);
            Debug.Log("Combo score: " + comboScore + " bonus ponts: " + comboPoints);
            score += comboPoints;
            comboText.text = (combo + "X Combo + " + bonus + "%");
            uiScript.AddToComboList("Combo bonus " + comboPoints);

            // Style bonus (# of Different tricks per combo)
            switch (combolist.Count)
            {
                case (1):
                    score += 10;
                    uiScript.AddToComboList("+" + 10 + " style points!");
                    Debug.Log("+" + 10 + " style points!");
                    break;
                case (2):
                    score += 50;
                    uiScript.AddToComboList("+" + 50 + " style points!");
                    Debug.Log("+" + 50 + " style points!");
                    break;
                case (3):
                    score += 250;
                    uiScript.AddToComboList("+" + 250 + " style points!");
                    Debug.Log("+" + 250 + " style points!");
                    break;
                case (4):
                    score += 1000;
                    uiScript.AddToComboList("+" + 1000 + " style points!");
                    Debug.Log("+" + 1000 + " style points!");
                    break;
                case (5):
                    score += 5000;
                    uiScript.AddToComboList("+" + 5000 + " style points!");
                    Debug.Log("+" + 5000 + " style points!");
                    break;
                case (6):
                    score += 10000;
                    uiScript.AddToComboList("+" + 10000 + " style points!");
                    Debug.Log("+" + 10000 + " style points!");
                    break;
                case (7): // Dont think its possible but just in case
                    score += 10000;
                    uiScript.AddToComboList("+" + 10000 + " style points!");
                    Debug.Log("+" + 10000 + " style points!");
                    break;
                case (8): // Dont think its possible but just in case
                    score += 10000;
                    uiScript.AddToComboList("+" + 10000 + " style points!");
                    Debug.Log("+" + 10000 + " style points!");
                    break;
            }

            uiScript.AddMovesToComboList(movelist);
        }

        combo = 0;
        comboScore = 0;
        combolist.Clear();
        movelist.Clear();
    }
}
