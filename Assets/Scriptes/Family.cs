using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Family : MonoBehaviour
{
    #region Field

    public string[] objectiveStrings;
    private bool _textIsRed;
    [FormerlySerializedAs("ExclamationMarkWaiting")] public float exclamationMarkWaiting = 0.5f;
    public int maxExclamationMark;

    public GameObject chatBubble;
    public TextMeshPro textMy;
    private bool _emptyText;
    
    private Rigidbody2D _rigidbody;
    private Item _meAsAnItem;
    public GameManager gameManager;
    public Dragon dragon;
    public Player player;
    [FormerlySerializedAs("dadShouting")] public string dadShoutingString;
    private int _exclamationMark;
    public float timeToWait;

    private bool _sameRoomAsPlayer;
    private float _time;
    private Animator _animator;

    public TextMeshProUGUI text;

    [FormerlySerializedAs("_dadShouting")] public bool dadShouting;
    public bool insideTheHouse;
    private Subtitles _subtitles;

    public int numCam;
    private bool _encounterWasMade;
    public string[] firstEncounterStrings;
    public string[] lastEncounterStrings;
    
    public Transform houseWhereToPop;
    private bool _catchingMe;
    private static readonly int Rotate1 = Animator.StringToHash("rotate");

    #endregion
    
    #region MonoBehaviour
    private void Awake()
    {
        if (chatBubble)
            chatBubble.SetActive(false); 
        
        textMy = GetComponentInChildren<TextMeshPro>();
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _meAsAnItem = GetComponent<Item>();
        _animator = gameObject.GetComponent<Animator>();
        _animator.SetBool(Rotate1, true);
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

        
        Rotate((_subtitles.showStrings == -1 & !dadShouting) & !_catchingMe);

        if (!_encounterWasMade & _sameRoomAsPlayer)
        {
            
            dadShouting = true;
            _time += Time.deltaTime;
            if (_time >= exclamationMarkWaiting)
            {
                exclamationMarkWaiting = timeToWait;
                if (_exclamationMark == maxExclamationMark & !_textIsRed )
                {
                    textMy.color = Color.red;
                    _textIsRed = true;
                }
                if (_exclamationMark <= maxExclamationMark)
                {
                    var mark = MarkGenerator();
                    if (chatBubble)
                    {
                        chatBubble.SetActive(true);
                    }

                    textMy.text = dadShoutingString + mark;
                    _time = 0;
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
        dadShouting = false;
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
        _animator.SetBool(Rotate1, state);
    }

    /**
     * ExclamationMark generator for the shouting of the family members.
     */
    private string MarkGenerator()
    {
       
        var mark = "";
        for (var i = 0; i < _exclamationMark; i++)
        {
            mark += "!";
        }

        _exclamationMark++;
        return mark;
    }
    
    #endregion
}