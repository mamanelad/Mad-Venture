using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SwordBonuse : MonoBehaviour
{
    #region  Fields

    public String objectiveString;
    public TextMeshProUGUI text;
    public string[] firstEncounterStrings;
    public float tineForSub;
    private string _name;
    
    private Subtitles _subtitles;
    
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        _name = gameObject.name;
        _subtitles = text.GetComponent<Subtitles>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<Player>();
        player.BuildSwordShooter();
        _subtitles.currentObjective = objectiveString;
        _subtitles.textCurrentObjective.text = objectiveString;
        _subtitles.textOriginal = true;
        _subtitles.currentStrings = firstEncounterStrings;
        _subtitles.timeToWait = tineForSub;
        _subtitles.showStrings = 0;
        Destroy(gameObject);
    }
    
    #endregion
}
