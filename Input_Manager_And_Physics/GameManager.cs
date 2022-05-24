using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Text scoreField;
    private int playerScore1 = 0;
    private int playerScore2 = 0;

    private void Awake()
    {
        scoreField.text = playerScore1 + " - " + playerScore2;
    }

    public void UpdateScore(int player)
    {
        if (player == 0)
        {
            playerScore1++;
        }
        else
        {
            playerScore2++;
        }
        scoreField.text = playerScore1 + " - " + playerScore2;
    }
}
