using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Triggers : MonoBehaviour
{
    #region Fields

    public bool playerIsOverlapping;
    public GameObject maze;
    public GameManager gameManager;
    public int numCam;
    public int numColor;
    public Player playerG;
    public Transform player;
    public Transform receiver;

    #endregion

    #region MonoBehaviour

    public void Update()
    {
        if (!playerIsOverlapping) return;
        Transfer();
    }

    /**
     * Transfer the items that the player is currently taking.
     */
    private void TransferItems(Item currentItem, Vector3 oldPlayerPosition)
    {
        var newPlayerPosition = playerG.transform.position;
        var transform1 = currentItem.transform;
        var position = transform1.position;
        var itemOldPosition = position;
        var distanceX = oldPlayerPosition.x - itemOldPosition.x;
        var distanceY = oldPlayerPosition.y - itemOldPosition.y;
        var newItemPosition = new Vector2(newPlayerPosition.x - distanceX, newPlayerPosition.y - distanceY);
        position = newItemPosition;
        position -= new Vector3(0, 0, 1);
        transform1.position = position;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsOverlapping = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsOverlapping = false;
        }
    }

    private void Transfer()
    {
        var oldPlayerPosition = playerG.transform.position;
        if (gameObject.name != "easterEgg")
        {
            var transform1 = transform;
            Vector2 portalToPlayer = player.position - transform1.position;
            var dotProduct = Vector2.Dot(transform1.up, portalToPlayer);
            if (!(dotProduct < 0)) return;
        }
        else
        {
            Destroy(gameObject);
        }

        var position = (Vector2) receiver.position;
        player.position = position;
        playerG.curCamara = numCam;
        gameManager.curCam = numCam;
        GameManager.SwitchCamara(numCam);
        player.GetComponent<SpriteRenderer>().color = playerG.Colors[numColor];
        maze.GetComponent<Tilemap>().color = playerG.Colors[numColor];
        playerIsOverlapping = false;
        foreach (var item in gameManager.items)
        {
            var hingeJoint2D = item.GetComponent<HingeJoint2D>();
            if (hingeJoint2D.enabled)
            {
                TransferItems(item, oldPlayerPosition);
            }
        }
    }

    #endregion
}