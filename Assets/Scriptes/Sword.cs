using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    #region public fields 
    
    public float speed = 20f;
    public int damage = 100;
    public Rigidbody2D swordRigidbody2D;
    public GameObject impactEffect;
    
    #endregion
    
    #region MonoBehaviour
    void Start()
    {
        swordRigidbody2D.velocity = transform.right * speed * (-1);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var dragon = other.GetComponent<Dragon>();
        if (dragon != null)
        {
            dragon.TakeDamage(damage);
        }

        var transform1 = transform;
        Instantiate(impactEffect, transform1.position, transform1.rotation);
        GameManager.PlaySound(6);
        Destroy(gameObject);
    }
    
    #endregion
}
