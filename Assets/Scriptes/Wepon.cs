using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Wepon : MonoBehaviour
{
    #region  Fields
    public float speed = 5f;
    public Transform firePoint;
    public GameObject swordPrefab;
    public GameManager gameManager;
    private bool _canShot = true;
    
    #endregion

    #region MonoBehaviour
    // Update is called once per frame
    public void Update()
    {
        //Get the Screen positions of the object
        var curCam = gameManager.cameras[gameManager.curCam];
        // Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);
        Vector2 positionOnScreen = curCam.WorldToViewportPoint(transform.position);

        //Get the Screen position of the mouse
        // Vector2 mouseOnScreen = (Vector2) Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Vector2 mouseOnScreen = (Vector2) curCam.ScreenToViewportPoint(Input.mousePosition);

        //Get the angle between the points
        float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);

        //Ta Daaa
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

        if (_canShot & Input.GetKey(KeyCode.Mouse0))
        {
            _canShot = false;
            Shoot();
        }

        if (!_canShot & !(Input.GetKey(KeyCode.Mouse0)))
        {
            _canShot = true;
        }
    }

    /**
     * Shot the sword.
     */
    private void Shoot()
    {
        var x = Instantiate(swordPrefab, firePoint.position, firePoint.rotation);
    }

    /**
     * Calculate the angle to shot the sword.
     */
    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
    
    #endregion
}