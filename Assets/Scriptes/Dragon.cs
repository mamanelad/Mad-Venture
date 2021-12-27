using System;
using System.Collections;
using UnityEngine;

public class Dragon : MonoBehaviour
{
    #region Fields

    //Other Game objects
    public GameObject player;
    public Player PlayerP;
    public GameObject mouth;

    //Patrol
    public bool patrol;
    public Transform[] patrolPoints; // Patrol points for the dragon to patrol 
    private int _currentPatrolIndex = 0;
    private Transform _currentPatrolPoint;

    //Components
    private Animator _animator;
    public Rigidbody2D rBDragon;
    private PolygonCollider2D _mouthCollider;
    private SpriteRenderer _spriteRenderer;

    // List of Components
    public Sprite[] sprites;
    public PolygonCollider2D[] polygonCollider2Ds;

    // Dragon mouth coordinates
    public float toDragonMouthX;
    public float toDragonMouthY;

    //General
    public int curCamara;
    public bool metPlayer = false;

    #endregion

    #region Movement

    private bool _returnMovement = false;
    public bool dragonIsMoving = false;
    public float timeToWait;
    private Vector2 _movement;
    public float moveSpeed;

    #endregion

    #region Player attacking

    public int health = 100;
    public bool dragonIsDead = false;
    public bool playerIsDead = false;
    private bool _mouthIsOpen = false;

    #endregion

    #region status

    private int _currentStage = 0;
    private int _nextStage;
    private bool _changeState = false;

    #endregion

    #region Animation

    public GameObject poof;
    public float rotateFrom;
    private static readonly int Rotate1 = Animator.StringToHash("rotate");

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        // Setting the first patrol point
        if (patrol)
            _currentPatrolPoint = patrolPoints[_currentPatrolIndex];

        //Setting game object components
        _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer.sprite = sprites[0];
        _animator = gameObject.GetComponentInChildren<Animator>();
        rBDragon = gameObject.GetComponent<Rigidbody2D>();
        _mouthCollider = mouth.GetComponent<PolygonCollider2D>();
        _mouthCollider.isTrigger = true;
        _mouthCollider.enabled = false;


        //Disabling all the not first polygonCollider2Ds 
        foreach (var polygonCollider2D in polygonCollider2Ds)
            polygonCollider2D.enabled = false;
        polygonCollider2Ds[0].enabled = true;
    }


    private void FixedUpdate()
    {
        if (dragonIsDead)
            killDragon();

        if (metPlayer)
            Rotate();

        if (patrol & !metPlayer)
            Patrol();

        if (dragonIsMoving & metPlayer)
            MoveToPlayer(_movement);

        if (_changeState)
            StartCoroutine(WaitToStateChange(timeToWait));
    }

    private void killDragon()
    {
        
        
    }
    


    /**
     * Moving the dragon to the player
     */
    private void MoveToPlayer(Vector2 direction)
    {
        rBDragon.MovePosition((Vector2) gameObject.transform.position + (direction * moveSpeed * Time.deltaTime));
        var newP = rBDragon.position;
        rBDragon.velocity = Vector2.zero;
        Vector2 directionToChangeTo = player.transform.position - transform.position;
        directionToChangeTo.Normalize();
        _movement = directionToChangeTo;
    }

    /**
     * Moving the dragon on patrol 
     */
    private void Patrol()
    {
        MoveToPlayer(_movement);
        rBDragon.velocity = Vector2.zero;
        Vector2 direction = patrolPoints[_currentPatrolIndex].position - transform.position;
        direction.Normalize();
        _movement = direction;

        if (!(Vector2.Distance(transform.position, _currentPatrolPoint.position) < 1.0f)) return;
        _currentPatrolIndex = ((_currentPatrolIndex + 1) % patrolPoints.Length);
        _currentPatrolPoint = patrolPoints[_currentPatrolIndex];
    }


    /**
     * Calling the rotate dragon animation if needed.
     */
    private void Rotate()
    {
        var dicY = Math.Abs(_movement.y);
        if (dragonIsMoving & (dicY <= rotateFrom) & (dicY >= -rotateFrom))
            _animator.SetBool(Rotate1, true);
        else
            _animator.SetBool(Rotate1, false);
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            CollisionAndTrigger("Player");
    }
    
    
    /**
     * Setting the right thing to do depends on which gameobjects the dragon
     * encounteras with.
     */
    private void CollisionAndTrigger(string nameO)
    {
        if (_currentStage == 0 & !playerIsDead)
        {
            dragonIsMoving = false;
            _changeState = true;
            switch (nameO)
            {
                case "Player":
                    _nextStage = 1;
                    _mouthCollider.enabled = true;
                    _mouthIsOpen = true;
                    GameManager.PlaySound(2);
                    break;
                case "Sword":
                    _nextStage = 2;
                    GameManager.PlaySound(5);
                    break;
            }
        }
    }


    /**
     * Waiting before state changing 
     */
    private IEnumerator WaitToStateChange(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        ChangeDragonState();
    }


    /**
     * Setting all the values to be accourate,
     * after the dragon ate the player.
     */
    public void EatPlayer()
    {
        GameManager.PlaySound(4);
        _currentStage = 1;
        _nextStage = 0;
        _changeState = true;
        playerIsDead = true;
        dragonIsMoving = false;
        player.layer = 15;
    }


    /**
     * Returning the dragon back to Normal Mode
     */
    public void ReturnNormal()
    {
        _currentStage = 1;
        _nextStage = 0;
        _mouthCollider.enabled = false;
        _changeState = true;
        _returnMovement = true;
    }


    /**
     * Changing dragon mode :
     * sprite, colider , position and values
     */
    private void ChangeDragonState()
    {
        _changeState = false;
        var toRemove = polygonCollider2Ds[_currentStage];
        toRemove.enabled = false;
        _currentStage = _nextStage;
        var changeTo = polygonCollider2Ds[_currentStage];
        changeTo.enabled = true;
        _spriteRenderer.sprite = sprites[_currentStage];


        if (_returnMovement)
        {
            dragonIsMoving = true;
            _returnMovement = false;
        }


        var newPosition = player.GetComponent<Rigidbody2D>().position;
        if (_mouthIsOpen)
        {
            newPosition.x += toDragonMouthX;
            newPosition.y -= toDragonMouthY;
            rBDragon.position = newPosition;
            _mouthIsOpen = false;
        }

        if (_currentStage == 2)
            dragonIsDead = true;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            CollisionAndTrigger("Sword");
        }
    }

    #endregion
}