using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Triggers : MonoBehaviour
{
    #region Fields

    public GameObject maze;
    public GameManager gameManager;
    public int numCam;
    public int numColor;
    public Player playerG;
    public Transform player;
    public Transform receiver;
    private bool _playerIsOverlapping;

    #endregion
    
    #region MonoBehaviour
    // Update is called once per frame
    public void Update()
    {
        if (!_playerIsOverlapping) return;
        var oldPlayerPosition = playerG.transform.position;
        var transform1 = transform;
        Vector2 portalToPlayer = player.position - transform1.position;
        var dotProduct = Vector2.Dot(transform1.up, portalToPlayer);

        if (!(dotProduct < 0)) return;
        var difference = playerG.direction * playerG.size * 2;
        // var position = player.position;
        // position = (Vector2) receiver.position + difference ;
        // position -= new Vector3 (0, 0, 1);
        var position = (Vector2) receiver.position; /////
        player.position = position;
        playerG.curCamara = numCam;
        gameManager.curCam = numCam;
        GameManager.SwitchCamara(numCam);
        player.GetComponent<SpriteRenderer>().color = playerG.Colors[numColor];
        maze.GetComponent<Tilemap>().color = playerG.Colors[numColor];
        _playerIsOverlapping = false;
        foreach (var item in gameManager.items)
        {
            var hingeJoint2D = item.GetComponent<HingeJoint2D>();
            if (hingeJoint2D.enabled)
            {
                TransferItems(item, oldPlayerPosition);
            }
        }
    }
    
    private void TransferItems(Item currentItem, Vector3 oldPlayerPosition)
    {
        var newPlayerPosition = playerG.transform.position;
        var transform1 = currentItem.transform;
        var position = transform1.position;
        var itemOldPosition = position;
        var distanceX = oldPlayerPosition.x - itemOldPosition.x;
        var distanceY = oldPlayerPosition.y - itemOldPosition.y;
        var newItemPosition = new Vector2(newPlayerPosition.x  - distanceX, newPlayerPosition.y - distanceY );
        position = newItemPosition;
        position -= new Vector3 (0, 0, 1);
        transform1.position = position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerIsOverlapping = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerIsOverlapping = false;
        }
    }
    
    #endregion
}