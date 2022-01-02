using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Subtitles : MonoBehaviour
{
    // public GameObject textBox;

    public Player Player;
    private Text _text;
    // private int exclamationMark = 0;
    private string currentText = "";
    public float timeToWait;
    private float time;
    private bool firstMove = false;
    public bool catchFirstItem = false;
    private bool _firstItemReleasing = false;

    public bool dadShouting = false;


    public string[] currentStrings;
    public int showStrings = -1;
    private bool _coroutineIsRunning = false;
    

    // Start is called before the first frame update
    private void Awake()
    {
        _text = gameObject.GetComponent<Text>();
        _text.text = "Press 'W', 'A', 'S', 'D' to move ";

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
            StartCoroutine(ChangeText("Press space for releasing an item", 0.5f));
        }

        if (!_firstItemReleasing & Input.GetKey(KeyCode.Space))
        {
            Player.rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            _firstItemReleasing = true;
            StartCoroutine(ChangeText("", 0.5f));
        }
        
     
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        //
        //
        //
        // if (Player.canMove & firstMove & (_text.text == "" || _text.text.Contains("Dad")) )
        // {
        //     if (dadShouting)
        //     {
        //         time += Time.deltaTime;
        //         if (time >= timeToWait)
        //         {
        //             var mark = markGenerator();
        //             _text.text = "Dad" + mark;
        //             time = 0;
        //         }
        //     }
        //
        //     // else
        //     // {
        //     //     
        //     //     StartCoroutine(ChangeText("", 0.5f));
        //     // }
        // }
        //
        //


        if (showStrings != -1)
        {
            if (!_coroutineIsRunning)
            {
                
                StartCoroutine(ChangeText(currentStrings[showStrings], 0.5f));
                showStrings += 1;
            }
            
            if (showStrings == currentStrings.Length)
            {
                showStrings = -1;
            }
        }
    }

    //
    // public void FamilyEncounter(string[] strings)
    // {
    //     foreach (var str in strings)
    //     {
    //         print("KAKA");
    //         
    //     }
    // }




    IEnumerator ChangeText(string str, float time)
    {
        _coroutineIsRunning = true;
        yield return new WaitForSeconds(time);
        _text.text = str;
        _coroutineIsRunning = false;
    }
    
   
    
}