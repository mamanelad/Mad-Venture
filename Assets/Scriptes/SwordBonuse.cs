using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SwordBonuse : MonoBehaviour
{
    private string _name;
    public TextMeshProUGUI text;
    private Subtitles _subtitles;
    public string[] firstEncounterStrings;

    private void Awake()
    {
        _name = gameObject.name;
        _subtitles = text.GetComponent<Subtitles>();

        
    }

  

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<Player>();
        player.BuildSwordShooter();
        //TODO : call some animation and when animation is finise. 
        Destroy(gameObject);
        _subtitles.currentStrings = firstEncounterStrings;
        _subtitles.showStrings = 0;
    }
}
