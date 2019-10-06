﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RaceTrackBlurb : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Text blurb;
    public GameObject blurbObject;
    public GameObject panel;

    // Start is called before the first frame update
    void Start()
    {
        RaceTrack raceTrack = new RaceTrack();
        blurb.text = raceTrack.Blurb;
        panel.SetActive(false);
        blurbObject.SetActive(false);
    }
    
    public void OnPointerEnter(PointerEventData pointerEventData){
        panel.SetActive(true);
        blurbObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData){
        panel.SetActive(false);
        blurbObject.SetActive(false);
    }
    public void OnPointerClick(PointerEventData pointerEventData) {
        panel.SetActive(false);
        blurbObject.SetActive(false);
    }
}
