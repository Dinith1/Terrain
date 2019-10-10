using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndscreenController : MonoBehaviour
{
    public GameObject endScreenUI;

    public TextMeshProUGUI endText;

    void Update()
    {
        if (GameController.Instance.Game.IsEnd)
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        endScreenUI.SetActive(true);

        if (GameController.Instance.Game.IsVictory)
        {
            endText.text = "YOU WIN! GREEN POINT GOAL REACHED.";
        }
        else
        {
            if ((GameController.Instance.Game.CurrentTurn >= GameController.Instance.Game.MaxTurns) && (GameController.Instance.Game.Green < GameController.Instance.Game.MaxGreen))
            {
                endText.text = "YOU LOSE! NOT ENOUGH GREEN POINTS. THE WORLD IS RUINED!";
            }
            else
            {
                endText.text = "YOU LOSE! RAN OUT OF MONEY. THE WORLD IS RUINED!";
            }
        }
    }
}