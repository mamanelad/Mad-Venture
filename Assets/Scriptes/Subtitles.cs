using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Subtitles : MonoBehaviour
{
    #region GameObjects

    public GameObject swordBonus;
    public GameObject chatBubble = null;
    public Player player;

    #endregion
    
    #region public fields
    
    public String currentObjective;
    public float timeToWait;
    public TextMeshProUGUI text;
    public TextMeshPro curFamilyTextBubble = null;
    public TextMeshProUGUI textCurrentObjective;
    public bool textOriginal = false;
    public bool catchFirstItem = false;
    public bool dadShouting = false;
    public string[] currentStrings;
    public int showStrings = -1;
    
    #endregion

    #region private fields

    private float _releaseItemMessageWaitingTime = 0;
    private String lastStr = " ";
    private float time;
    private bool firstMove = false;
    private bool _firstItemReleasing = false;
    private bool _coroutineIsRunning = false;

    #endregion
    
    #region MonoBehaviour
    private void Awake()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {

        if (!firstMove & (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) ||
                          Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)))
        {
            firstMove = true;
            StartCoroutine(ChangeText("", 0.5f));
        }

        if (!catchFirstItem & player.currentItem != player.empty)
        {
            catchFirstItem = true;
            StartCoroutine(ChangeText("Press  <sprite name=Space_Key_Light> \n" + "to release an item", 0.1f));
            _releaseItemMessageWaitingTime = 0.1f;
        }

        if (_releaseItemMessageWaitingTime != 0)
        {
            _releaseItemMessageWaitingTime += Time.deltaTime;
        }

        if ((!_firstItemReleasing & Input.GetKey(KeyCode.Space)) | _releaseItemMessageWaitingTime >= 2)
        {
            _releaseItemMessageWaitingTime = 0;
            _firstItemReleasing = true;
            StartCoroutine(ChangeText("", 0.5f));
        }


        //change subtitles of family members
        if (showStrings != -1)
        {
            if (!_coroutineIsRunning)
            {
                StartCoroutine(showStrings == 0
                    ? ChangeText(currentStrings[showStrings], 0f)
                    : ChangeText(currentStrings[showStrings], timeToWait));

                showStrings += 1;
            }
        }
    }
    
    /**
     * change Subtitles of the gamwe
     */
    IEnumerator ChangeText(string str, float time)
    {
        _coroutineIsRunning = true;
        yield return new WaitForSeconds(time);
        if (str == "" & chatBubble != null)
            chatBubble.SetActive(false);
        

        if (curFamilyTextBubble != null & !textOriginal)
        {
            curFamilyTextBubble.color = showStrings != -1 ? Color.red : Color.black;
            curFamilyTextBubble.text = str;
        }
        else
        {
            text.text = str;
        }
        
        _coroutineIsRunning = false;
        if (showStrings == currentStrings.Length)
        {
            showStrings = -1;
            textCurrentObjective.text = currentObjective;
            if (textOriginal)
                textOriginal = false;
            
        }

        if (lastStr.Contains("Sword") & swordBonus)
            swordBonus.SetActive(true);
        
        lastStr = str;
    }
    
    #endregion
}