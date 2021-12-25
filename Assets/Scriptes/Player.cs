using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class Player : MonoBehaviour
{
    #region Fields

    public GameManager gameManager;
    public SpriteRenderer spriteRenderer;
    public int color = 0;
    public GameObject[] mazeTriggers;
    private Rigidbody2D _rigidbody2D;
    public int curCamara;

    public readonly Color[] Colors = new[]
        {Color.yellow, Color.green, Color.red, Color.blue, Color.black, Color.magenta};

    #endregion

    #region Eating by a Dragon

    private float _gotInSideMouthTimer = 0;
    private float _continuumTimeInMouthTimer;
    private bool _gotInSideMouth = false;
    private bool _inSideMouth = false;
    public float deadTime;
    public GameObject[] dragonBalys;

    #endregion

    #region Movement

    public float timeToWaitAfterDragonAttack;
    public float size;
    public Vector2 direction;
    public float slowPlayer;
    private bool _canMove = true;
    private bool _leftPressed;
    private bool _rightPressed;
    private bool _upPressed;
    private bool _downPressed;
    private bool _moveAfterTransfer = true;
    private bool _inSwordRoom = false;
    private float _movingTimer;
    private bool _longClicking = false;

    #endregion

    #region Items

    public GameObject empty;
    public GameObject currentItem;
    public bool withItem = false;
    public bool withMoveThrow = false;

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        currentItem = empty;
        Application.targetFrameRate = 70;
        _rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        size = _rigidbody2D.transform.localScale.x / 2;
        spriteRenderer.color = Colors[color];
        gameManager.tilemap.color = Colors[color];
    }

    private void FixedUpdate()
    {
        if (!_canMove) return;
        var ultimate = Vector2.zero;

        if (_leftPressed)
            ultimate += Vector2.left;
        if (_rightPressed)
            ultimate += Vector2.right;
        if (_upPressed)
            ultimate += Vector2.up;
        if (_downPressed)
            ultimate += Vector2.down;


        _longClicking = (_leftPressed | _rightPressed | _upPressed | _downPressed);
        var oldTimer = _movingTimer;
        if (_longClicking)
            _movingTimer += Time.deltaTime;
        else
            _movingTimer = 0;

        direction = ultimate;
        if (!(oldTimer == 0 | _movingTimer >= slowPlayer)) return;
        _movingTimer = 0.001f;
        _rigidbody2D.MovePosition(_rigidbody2D.position + size * ultimate);
    }

    private void Update()
    {
        _rigidbody2D.velocity = Vector2.zero;
        _leftPressed = Input.GetKey(KeyCode.LeftArrow);
        _rightPressed = Input.GetKey(KeyCode.RightArrow);
        _upPressed = Input.GetKey(KeyCode.UpArrow);
        _downPressed = Input.GetKey(KeyCode.DownArrow);


        if (!_moveAfterTransfer)
            StartCoroutine(WaitAfterTransfer());


        if (_gotInSideMouth)
        {
            _gotInSideMouthTimer += Time.deltaTime;
            _continuumTimeInMouthTimer += Time.deltaTime;
        }

        if (_gotInSideMouthTimer >= deadTime)
        {
            if (_inSideMouth & _continuumTimeInMouthTimer >= deadTime)
            {
                GameManager.EatPlayer();
                TransferToBally();
            }

            else
            {
                GameManager.DragonReturnNormal();
            }

            _gotInSideMouthTimer = 0;
            _gotInSideMouth = false;
        }

        if (!_canMove)
            StartCoroutine(Wait(timeToWaitAfterDragonAttack));
    }

    /**
 * Waiting after transfer 
 */
    private IEnumerator WaitAfterTransfer()
    {
        yield return new WaitForSeconds(0.1f);
        _moveAfterTransfer = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Dragon") & _canMove)
            _canMove = false;
    }

    /**
     * Waiting some amount of time.
     */
    private IEnumerator Wait(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        if (!_canMove)
            _canMove = true;
    }


    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!_moveAfterTransfer) return;
        var flag = false;
        var otherGameObject = other.gameObject;
        var otherPosition = otherGameObject.transform.position;
        var otherPositionY = otherPosition.y;
        var otherPositionX = otherPosition.x;

        var myPosition = transform.position;
        var myPositionY = myPosition.y;
        var myPositionX = myPosition.x;

        curCamara = 0;

        switch (otherGameObject.name)
        {
            case "triggerYellowCasttleRoom":
                flag = true;
                curCamara = 1;
                color = 0;
                TransferPlayer(0, false, false);
                other.enabled = true;
                break;
            case "triggerBlackCasttleRoom":
                // print("papa");
                flag = true;
                curCamara = 12;
                color = 2;
                TransferPlayer(17, false, false);
                other.enabled = true;
                break;

            case "triggerMagnetaRoom":
                flag = true;
                curCamara = 11;
                color = 4;
                TransferPlayer(18, false, false);
                break;
        }


        if (flag)
        {
            _moveAfterTransfer = false;
            GameManager.SwitchCamara(curCamara);
            spriteRenderer.color = Colors[color];
            gameManager.tilemap.color = Colors[color];
        }


        if (other.gameObject.CompareTag("moveThrow"))
        {
            if (!withMoveThrow)
            {
                gameObject.layer = 14;
                _rigidbody2D.constraints =
                    RigidbodyConstraints2D.FreezePositionX  | RigidbodyConstraints2D.FreezeRotation;
            }
        }

        if (other.gameObject.name == "BridgeLeft" || other.gameObject.name == "BridgeRight")
            withMoveThrow = true;
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name != "mouthTrigger") return;
        _inSideMouth = true;
        _gotInSideMouth = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "mouthTrigger")
        {
            _continuumTimeInMouthTimer = 0f;
            _inSideMouth = false;
        }


        if (other.gameObject.CompareTag("moveThrow"))
        {
            gameObject.layer = 7;
            _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        }


        if (!_moveAfterTransfer) return;
        var flag = false;
        var otherGameObject = other.gameObject;
        var otherPosition = otherGameObject.transform.position;
        var otherPositionY = otherPosition.y;
        var otherPositionX = otherPosition.x;

        var myPosition = transform.position;
        var myPositionY = myPosition.y;
        var myPositionX = myPosition.x;
        switch (otherGameObject.name)
        {
            case "triggerSwordRoom":
                flag = true;
                curCamara = 0;
                color = 0;
                break;

            case "triggerYellowGreen":
            {
                flag = true;
                if (myPositionY >= otherPositionY)
                {
                    curCamara = 0;
                    color = 0;
                }

                else
                {
                    curCamara = 2;
                    color = 1;
                }

                break;
            }
            case "triggerGreenGreen1":
            {
                flag = true;
                curCamara = myPositionX >= otherPositionX ? 3 : 2;
                color = 1;
                break;
            }
            case "triggerGreenRed":
            {
                flag = true;
                if (myPositionY >= otherPositionY)
                {
                    curCamara = 3;
                    color = 1;
                }
                else
                {
                    curCamara = 4;
                    color = 2;
                }

                break;
            }
            case "triggerGreenGreen2":
                flag = true;
                curCamara = myPositionX <= otherPositionX ? 5 : 2;
                color = 1;
                break;
            case "triggerBlue3":
                flag = true;
                color = 3;
                curCamara = myPositionX <= otherPositionX ? 6 : 7;
                break;
            case "triggerBlue6":
                color = 3;
                flag = true;
                curCamara = myPositionX <= otherPositionX ? 10 : 9;
                break;
            case "triggerBlue7":
            {
                color = 3;
                flag = true;
                curCamara = myPositionY <= otherPositionY ? 9 : 7;

                break;
            }
            case "triggerBlue8":
            {
                flag = true;
                curCamara = myPositionY <= otherPositionY ? 7 : 8;
                break;
            }
            case "triggerTrophyRoom":
            {
                flag = true;
                if (myPositionY <= otherPositionY)
                {
                    curCamara = 12;
                    color = 2;
                }
                else
                {
                    curCamara = 13;
                    color = 5;
                }

                break;
            }

            case "triggerBlue17":
            {
                flag = true;
                if (myPositionY <= otherPositionY)
                {
                    curCamara = 8;
                    color = 3;
                }
                else
                {
                    curCamara = 11;
                    color = 4;
                }

                break;
            }
        }


        if (!flag) return;
        _moveAfterTransfer = false;
        GameManager.SwitchCamara(curCamara);
        spriteRenderer.color = Colors[color];
        gameManager.tilemap.color = Colors[color];
    }


    /**
 * Transferring the player to the  to the ask location.
 */
    private void TransferPlayer(int num, bool stayX, bool stayY)
    {
        var newPosition = mazeTriggers[num].transform.position;
        var position = transform.position;
        var positionX = position.x;
        var positionY = position.y;
        var newY = newPosition.y;
        var newX = newPosition.x;
        switch (num == 0)
        {
            case true:
                newX = positionX;
                newY += (3 * size);
                _inSwordRoom = true;
                break;
        }

        if (num == 1 & num == 16)
        {
            newX = positionX;
            if (_inSwordRoom)
            {
                newX += 2 * size;
                newY += (6 * size);
                _inSwordRoom = false;
            }
        }

        if (stayX)
        {
            newX = positionX;
        }

        if (stayY)
        {
            newY = positionY;
        }


        _rigidbody2D.position = new Vector2(newX, newY) + (direction.normalized);

        if (withItem)
        {
            ChangeItemPosition(currentItem, positionX, positionY, newX, newY);
        }
    }

    /**
 * Transfer the current item to the ask location
 */
    private static void ChangeItemPosition(GameObject other, float oldX, float oldY, float newX, float newY)
    {
        var currentItemPosition = other.transform.position;
        var distanceX = oldX - currentItemPosition.x;
        var distanceY = oldY - currentItemPosition.y;
        currentItemPosition = new Vector2(newX - distanceX, newY - distanceY);
        other.GetComponent<Rigidbody2D>().MovePosition(currentItemPosition);
    }

    /**
 * Transfer the player to the dragon Bally.
 */
    private void TransferToBally()
    {
        var dragonBallyPosition0 = dragonBalys[0].transform.position;
        var dragonBallyPosition1 = dragonBalys[1].transform.position;
        var playerNextPosition = dragonBallyPosition0;
        var oldPosition = _rigidbody2D.position;
        var d0 = Vector2.Distance(oldPosition, dragonBallyPosition0);
        var d1 = Vector2.Distance(oldPosition, dragonBallyPosition1);
        if (d1 < d0)
            playerNextPosition = dragonBallyPosition1;

        _rigidbody2D.transform.position = (playerNextPosition);
        if (!withItem) return;
        var transform1 = _rigidbody2D.transform;
        var dX = transform1.position.x - oldPosition.x;
        var dY = transform1.position.y - oldPosition.y;
        var position = currentItem.transform.position;
        var newItemPosition = new Vector2(position.x + dX, position.y + dY);
        position = newItemPosition;
        currentItem.transform.position = position;
    }

    #endregion
}