using System;
using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    #region public fields
    
    public Player player;
    public Animator animator;
    public float distanceFromPlayer;
    public GameManager gameManager;

    #endregion

    #region private fields

    private Rigidbody2D _rigidbody2D;
    public HingeJoint2D _hingeJoint2D;
    private SpriteRenderer _spriteRenderer;
    private string _itemName;
    private static readonly int SwitchColor = Animator.StringToHash("switchColor");

    #endregion
    
    #region MonoBehaviour

    private void Start()
    {
        // Set the joint between the key and the player to false
        _itemName = gameObject.name;
        animator = gameObject.GetComponent<Animator>();
        _rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        _hingeJoint2D = gameObject.GetComponent<HingeJoint2D>();
        _hingeJoint2D.enabled = false;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        GameManager.PlaySound(0);
        if (gameObject.CompareTag("family"))
            GameManager.ItemsBlinking(true);
       

        ReleaseItem();
        player.withItem = true;
        player.currentItem = gameObject;
        _rigidbody2D.MovePosition(_rigidbody2D.position + (distanceFromPlayer * player.size 
                                                                              *  player.direction.normalized));
        StartCoroutine(WaitToHinge());
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
            ReleaseItem();
    }

    private void ReleaseItem()
    {
        GameManager.PlaySound(1);
        var currentItem = player.currentItem.gameObject;
        foreach (var item in gameManager.items)
        {
            if (currentItem.CompareTag("family"))
                GameManager.ItemsBlinking(false);

            currentItem.GetComponent<HingeJoint2D>().enabled = false;
            currentItem.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.withItem = false;

            if (_hingeJoint2D.enabled)
            {
                _hingeJoint2D.enabled = false;
                _rigidbody2D.velocity = Vector2.zero;
            }

            player.currentItem = player.empty;
        }
    }

    private IEnumerator WaitToHinge()
    {
        yield return new WaitForSeconds(0.01f);
        _hingeJoint2D.enabled = true;
    }

    #endregion
}