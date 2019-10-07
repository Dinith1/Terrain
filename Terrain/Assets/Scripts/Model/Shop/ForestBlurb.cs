﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ForestBlurb : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Text blurb;
    public GameObject blurbObject;
    public Text metrics;
    public GameObject metricsObject;
    public GameObject panel;

    // Start is called before the first frame update
    void Start()
    {
        Forest forest = new Forest();
        blurb.text = forest.Blurb;
        metrics.text = "Cost: " + forest.InitialBuildMoney.ToString() +
        "\n\nInitial Green Points: " + forest.InitialBuildGreen.ToString() +
        "\nInitial Happiness: " + forest.InitialBuildHappiness.ToString() +
        "\n\nMoney per turn: " + forest.GenerateMoney.ToString() +
        "\nGreen Points per turn: " + forest.GenerateGreen.ToString() +
        "\nHappiness per turn: " + forest.GenerateHappiness.ToString();
        metricsObject.SetActive(false);
        panel.SetActive(false);
        blurbObject.SetActive(false);
    }
    
    public void OnPointerEnter(PointerEventData pointerEventData){
        panel.SetActive(true);
        blurbObject.SetActive(true);
        metricsObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData){
        panel.SetActive(false);
        blurbObject.SetActive(false);
        metricsObject.SetActive(false);
    }
    public void OnPointerClick(PointerEventData pointerEventData) {
        panel.SetActive(false);
        blurbObject.SetActive(false);
        metricsObject.SetActive(false);
    }
}
