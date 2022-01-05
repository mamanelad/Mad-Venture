using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Subtitles : MonoBehaviour
{
    // public GameObject textBox;


    public  TextMeshProUGUI _text;
    public TextMeshPro curFamilyTextBubble = null;
    public GameObject chatBubble = null;
    public Player Player;
    // private Text _text;
    // private int exclamationMark = 0;
    // private string currentText = "";
    public float timeToWait;
    private float time;
    private bool firstMove = false;
    public bool catchFirstItem = false;
    private bool _firstItemReleasing = false;

    public bool dadShouting = false;


    public string[] currentStrings;
    public int showStrings = -1;
    private bool _coroutineIsRunning = false;

    public GameObject swordBonus;

    // Start is called before the first frame update
    private void Awake()
    {
        _text = gameObject.GetComponent<TextMeshProUGUI>();
        _text.text = "Press <sprite name=move> to move";

    }


    // Update is called once per frame
    void Update()
    {
        if (!firstMove & (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) ||
                          Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)))
        {
            firstMove = true;
            StartCoroutine(ChangeText("", 0.5f));
        }

        if (!catchFirstItem & Player.currentItem != Player.empty)
        {
            Player.rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            catchFirstItem = true;
            StartCoroutine(ChangeText("Press  <sprite name=space> to release an item", 0.5f));
        }

        if (!_firstItemReleasing & Input.GetKey(KeyCode.Space))
        {
            Player.rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            _firstItemReleasing = true;
            StartCoroutine(ChangeText("", 0.5f));
        }
        
     
        

        if (showStrings != -1)
        {
            
            if (!_coroutineIsRunning)
            {
                StartCoroutine(showStrings == 0
                    ? ChangeText(currentStrings[showStrings], 0f)
                    : ChangeText(currentStrings[showStrings], timeToWait));

                showStrings += 1;
                if (curFamilyTextBubble.text.Contains("sword"))
                {
                    if (swordBonus)
                    {
                        swordBonus.SetActive(true);
                    }
                    
                }
            }
            
         
        }
    }

   



    IEnumerator ChangeText(string str, float time)
    {
        
        _coroutineIsRunning = true;
        yield return new WaitForSeconds(time);
        if (str == "" & chatBubble != null)
        {
            chatBubble.SetActive(false);
        }
        if (curFamilyTextBubble != null)
        {
            curFamilyTextBubble.text = str;
        }
        else
        {
            _text.text = str;
        }
        
        _coroutineIsRunning = false;
        if (showStrings == currentStrings.Length)
        {
            showStrings = -1;
        }
    }
    
   
    
}