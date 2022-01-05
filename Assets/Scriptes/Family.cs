using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Family : MonoBehaviour
{
    private bool _speakings = false;

    public GameObject chatBubble;
    public TextMeshPro textMy;
    private bool _emptyText;


    private Rigidbody2D _rigidbody;
    private Item _meAsAnItem;
    public GameManager gameManager;
    public Dragon dragon;
    public Player player;
    public String dadShouting;
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
        {
           chatBubble.SetActive(false); 
        }
        
        textMy = GetComponentInChildren<TextMeshPro>();
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _meAsAnItem = GetComponent<Item>();
        _animator = gameObject.GetComponent<Animator>();
        _animator.SetBool("rotate", true);
        _subtitles = text.GetComponent<Subtitles>();
    }

    void Update()
    {
        


        if (player.curCamara == 1 & _catchingMe & !insideTheHouse)
            GotIntoTheHouse();

        if (_catchingMe & Input.GetKey(KeyCode.Space))
        {
            _catchingMe = false;
        }

        if (numCam == player.curCamara)
            _sameRoomAsPlayer = true;

        // Rotate((!_catchingMe || numCam == 1) & !_speakings);
        // Rotate((_subtitles.showStrings == -1) & !_speakings);
        Rotate((_subtitles.showStrings == -1 & !_dadShouting) & !_catchingMe);




        if (!_encounterWasMade & _sameRoomAsPlayer)
        {
            _dadShouting = true;
            _speakings = true;
            time += Time.deltaTime;
            if (time >= timeToWait)
            {
                var mark = markGenerator();
                if (chatBubble)
                {
                   chatBubble.SetActive(true); 
                }
                
                textMy.text = dadShouting + mark;
                time = 0;
            }
        }


        if (_sameRoomAsPlayer & numCam != player.curCamara & !_encounterWasMade)
        {
            textMy.text = " ";
            chatBubble.SetActive(false);
            _sameRoomAsPlayer = false;
            _speakings = false;

        }
    }

    private void GotIntoTheHouse()
    {
        insideTheHouse = true;
        gameManager.familyMembersInTheHouse += 1;
        _meAsAnItem._hingeJoint2D.enabled = false;
        _rigidbody.MovePosition(houseWhereToPop.position);
        _rigidbody.velocity = Vector2.zero;
        _catchingMe = false;
        chatBubble.SetActive(true);
        _subtitles.currentStrings = lastEncounterStrings;
        _subtitles.showStrings = 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _dadShouting = false;
        _catchingMe = true;
        if (_encounterWasMade) return;
        _encounterWasMade = true;
        _subtitles.dadShouting = false;


        _subtitles.curFamilyTextBubble = textMy;
        _subtitles.currentStrings = firstEncounterStrings;
        _subtitles.showStrings = 0;

        _subtitles.chatBubble = chatBubble;


        if (gameObject.name == "Son")
        {
            _catchingMe = false;
            var boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
            boxCollider2D.enabled = false;
        }
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