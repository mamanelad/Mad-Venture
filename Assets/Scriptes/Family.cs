using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Family : MonoBehaviour
{
    public string[] objectiveStrings;
    private bool textIsRed = false;
    private float ExclamationMarkWaiting = 0.5f;
    public int maxExclamationMark;

    public GameObject chatBubble;
    public TextMeshPro textMy;
    private bool _emptyText;


    private Rigidbody2D _rigidbody;
    private Item _meAsAnItem;
    public GameManager gameManager;
    public Dragon dragon;
    public Player player;
    public string dadShouting;
    private int exclamationMark = 0;
    public float timeToWait;

    private bool _sameRoomAsPlayer;
    private float time;
    private Animator _animator;

    public TextMeshProUGUI text;

    public bool _dadShouting;
    public bool insideTheHouse;
    private Subtitles _subtitles;

    public int numCam;
    private bool _encounterWasMade = false;
    public string[] firstEncounterStrings;
    public string[] lastEncounterStrings;


    public Transform houseWhereToPop;
    private bool _catchingMe = false;

    private void Awake()
    {
        if (chatBubble)
            chatBubble.SetActive(false); 
        
        textMy = GetComponentInChildren<TextMeshPro>();
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _meAsAnItem = GetComponent<Item>();
        _animator = gameObject.GetComponent<Animator>();
        _animator.SetBool("rotate", true);
        _subtitles = text.GetComponent<Subtitles>();
    }


    private void Update()
    {
        if (insideTheHouse)
        {
            _meAsAnItem._hingeJoint2D.enabled = false;
            _rigidbody.MovePosition(houseWhereToPop.position);
        }

        if (player.curCamara == 1 & _catchingMe & !insideTheHouse)
            GotIntoTheHouse();

        if (_catchingMe & Input.GetKey(KeyCode.Space))
        {
            _catchingMe = false;
        }

        if (numCam == player.curCamara)
            _sameRoomAsPlayer = true;

        
        Rotate((_subtitles.showStrings == -1 & !_dadShouting) & !_catchingMe);

        if (!_encounterWasMade & _sameRoomAsPlayer)
        {
            
            _dadShouting = true;
            time += Time.deltaTime;
            if (time >= ExclamationMarkWaiting)
            {
                ExclamationMarkWaiting = timeToWait;
                if (exclamationMark == maxExclamationMark & !textIsRed )
                {
                    textMy.color = Color.red;
                    textIsRed = true;
                }
                if (exclamationMark <= maxExclamationMark)
                {
                    var mark = MarkGenerator();
                    if (chatBubble)
                    {
                        chatBubble.SetActive(true);
                    }

                    textMy.text = dadShouting + mark;
                    time = 0;
                }
            }
        }


        if (!(_sameRoomAsPlayer & numCam != player.curCamara & !_encounterWasMade)) return;
        textMy.text = " ";
        chatBubble.SetActive(false);
        _sameRoomAsPlayer = false;
    }

    /**
     * Transfer the family member to the right place in the house,
     * and setting the subtitles. 
     */
    private void GotIntoTheHouse()
    {
        insideTheHouse = true;
        gameManager.familyMembersInTheHouse += 1;
        _meAsAnItem._hingeJoint2D.enabled = false;
        _rigidbody.MovePosition(houseWhereToPop.position);
        _rigidbody.velocity = Vector2.zero;
        _catchingMe = false;
        
        if (lastEncounterStrings.Length <= 0) return;
        chatBubble.SetActive(true);
        
        _subtitles.currentStrings = lastEncounterStrings;
        _subtitles.showStrings = 0;
        player.currentItem = player.empty;
        
        _subtitles.currentObjective = objectiveStrings[1];
        _subtitles.textCurrentObjective.text = objectiveStrings[1];
        foreach (var item in gameManager.items)
        {
            if (item.carryMe)
            {
                player.currentItem = item.gameObject;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _dadShouting = false;
        _catchingMe = true;
         
        if (_encounterWasMade) return;
        _encounterWasMade = true;
        _subtitles.dadShouting = false;
        
        _subtitles.currentObjective = objectiveStrings[0];
        _subtitles.curFamilyTextBubble = textMy;
        _subtitles.currentStrings = firstEncounterStrings;
        _subtitles.showStrings = 0;
        _subtitles.timeToWait = timeToWait;

        _subtitles.chatBubble = chatBubble;
        
        if (gameObject.name != "Son") return;
        _catchingMe = false;
        var boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        boxCollider2D.enabled = false;
    }

    private void Rotate(bool state)
    {
        _animator.SetBool("rotate", state);
    }

    /**
     * ExclamationMark generator for the shouting of the family members.
     */
    private string MarkGenerator()
    {
       
        var mark = "";
        for (var i = 0; i < exclamationMark; i++)
        {
            mark += "!";
        }

        exclamationMark++;
        return mark;
    }
}