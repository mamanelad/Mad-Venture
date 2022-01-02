using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Family : MonoBehaviour
{
    public Player player;
    public String dadShouting;
    private int exclamationMark = 0;
    public float timeToWait;

    private bool _sameRoomAsPlayer;
    private float time;
    private Animator _animator;


    public bool _dadShouting;
    public Text text;
    private Subtitles _subtitles;

    public int numCam;
    private bool _encounterWasMade = false;
    public string[] firstEncounterStrings;


    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
        _animator.SetBool("rotate", true);
        _subtitles = text.GetComponent<Subtitles>();
    }

    void Update()
    {
        if (numCam == player.curCamara)
            _sameRoomAsPlayer = true;

        Rotate(player.currentItem != gameObject);
        if (!_encounterWasMade &_sameRoomAsPlayer)
        {
            time += Time.deltaTime;
            if (time >= timeToWait)
            {
                var mark = markGenerator();
                text.text = dadShouting + mark;
                time = 0;
            }
        }

        if (_sameRoomAsPlayer & numCam != player.curCamara & !_encounterWasMade)
        {
            text.text = "";
            _sameRoomAsPlayer = false;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        _encounterWasMade = true;
        _subtitles.dadShouting = false;
        _subtitles.currentStrings = firstEncounterStrings;
        _subtitles.showStrings = 0;
    }

    private void Rotate(bool state)
    {
        _animator.SetBool("rotate", state);
    }

    private String markGenerator()
    {
        var mark = "";
        for (int i = 0; i < exclamationMark; i++)
        {
            mark += "!";
        }

        exclamationMark++;
        return mark;
    }
}