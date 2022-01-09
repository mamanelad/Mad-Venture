using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    #region level managment

    public GameOverScreen gameOverScreen;
    public GameOverScreen gameWonScreen;

    #endregion

    #region UnityEvent

    public UnityEvent yellowSwordRoom;
    public UnityEvent openGateYellow;
    public UnityEvent blackTrophyRoom;
    public UnityEvent dragonBackToNormal;
    public UnityEvent eatPlayer;

    #endregion

    #region private Fields

    private static GameManager _shared;
    [FormerlySerializedAs("_curCam")] public int curCam; //The current camara.
    private bool _exitGame;
    private float _gameExitCounter;
    private static readonly int SwitchColor = Animator.StringToHash("switchColor");

    #endregion

    #region public Fields

    public int familyMembersInTheHouse;
    public GameObject swordRoom;
    public Camera[] cameras;
    public Item[] items;
    public Dragon[] dragons;
    public Player player;
    public AudioSource[] sounds;
    public Tilemap tilemap;

    #endregion

    #region MonoBehaviour

    public void Awake()
    {
        _shared = this;
        //Setting all the cameras beside the first one to not work. 
        for (var i = 1; i < cameras.Length; i++)
            cameras[i].enabled = false;

        //setting game exit counter equal to the finish sound time length.  
        _gameExitCounter = sounds[3].clip.length;
        tilemap = gameObject.GetComponentInChildren<Tilemap>();
    }

    /**
     * Calling the Dragon Patrol function if needed.
     */
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenue");
        }

        if (_shared.familyMembersInTheHouse == 3 & !_exitGame)
            PlaySound(3);


        foreach (var dragon in dragons)
        {
            if (dragon.patrol & !dragon.metPlayer)
                DragonPatrol(dragon);
        }

        if (_exitGame)
        {
            _gameExitCounter -= Time.deltaTime;
            _shared.player.transform.position = _shared.player.homeWhereToPop.position;
            _shared.player.rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }


        if (_gameExitCounter <= 0)
        {
            // _shared.player.rigidbody2D.MovePosition(_shared.player.homeWhereToPop.position);

            gameWonScreen.SetUp();
        }
    }


    /**
     * Called if a sound need to be play.
     * Plat the sound.
     */
    public static void PlaySound(int num)
    {
        if (num == 1 & _shared.player.currentItem == _shared.player.empty) return;
        _shared.sounds[num].Play();
        if (num != 3) return;
        _shared._exitGame = true;
        var animator = _shared.swordRoom.GetComponent<Animator>();
        animator.SetTrigger(SwitchColor);
    }

    /**
     * Called when the player goes to a different room and camara change is needed.
     */
    public static void SwitchCamara(int num)
    {
        _shared.player.curCamara = num;
        switch (num)
        {
            case 1:
                _shared.openGateYellow.Invoke();
                break;
            case 12:
                _shared.blackTrophyRoom.Invoke();
                break;
        }

        _shared.cameras[_shared.curCam].enabled = false;
        _shared.curCam = num;
        _shared.cameras[_shared.curCam].enabled = true;
        DragonManageCamara(num, 0); // just for the red dragon
        DragonManageCamara(num, 2); // just for the white dragon
    }

    /**
     * Setting which camara is the patrolling dragon is in.
     */
    private void DragonPatrol(Dragon curDragon)
    {
        GameObject target = curDragon.gameObject;
        for (var i = 0; i < cameras.Length; i++)
        {
            var cam = cameras[i];
            if (IsVisible(cam, player.gameObject) & player.curCamara != i)
            {
                cameras[player.curCamara].enabled = false;
                player.curCamara = i;
                cameras[player.curCamara].enabled = true;
            }

            if (!IsVisible(cam, target)) continue;
            curDragon.curCamara = i;
            if (curDragon.curCamara != _shared.player.curCamara) continue;
            DragonManageCamara(i, 1);
            break;
        }
    }

    /**
     * Setting the eat player event.
     */
    public static void EatPlayer()
    {
        _shared.player.currentDragon.GetComponent<Dragon>().EatPlayer();
    }

    /**
     * Setting the dragon return normal event.
     */
    public static void DragonReturnNormal()
    {
        _shared.dragonBackToNormal.Invoke();
    }

    /**
     * Setting which camara is the patrolling dragon is in.
     */
    private static void DragonManageCamara(int num, int dragonNum)
    {
        var curDragon = _shared.dragons[dragonNum];
        if (!curDragon) return;
        if (curDragon.curCamara == num)
        {
            curDragon.patrol = false;
            curDragon.metPlayer = true;
            curDragon.dragonIsMoving = true;
        }


        else
        {
            if (!(curDragon.CompareTag("Dragon") & curDragon.metPlayer & !curDragon.dragonIsDead)) return;
            if (!_shared.player.withItem & curDragon.dragonIsMoving)
            {
                curDragon.dragonIsMoving = false;
                curDragon.rBDragon.position = _shared.cameras[curDragon.curCamara].transform.position;
            }

            else
            {
                if (curDragon.dragonIsMoving)
                {
                    curDragon.curCamara = num;
                }
            }
        }
    }

    /**
     * Gets a camara game object and a dragon game object and returning true if the camara see the dragon.
     * Returning false otherwise.
     */
    private static bool IsVisible(Camera c, [NotNull] GameObject target)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        var planes = GeometryUtility.CalculateFrustumPlanes(c);
        var point = target.transform.position;
        foreach (var plane in planes)
        {
            if (plane.GetDistanceToPoint(point) < 0)
            {
                return false;
            }
        }

        return true;
    }

    public void FinishGame()
    {
        gameOverScreen.SetUp();
    }

    #endregion
}