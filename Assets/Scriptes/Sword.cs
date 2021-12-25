using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 100;

    public Rigidbody2D swordRigidbody2D;

    public GameObject impactEffect;
    // Start is called before the first frame update
    void Start()
    {
        swordRigidbody2D.velocity = transform.right * speed * (-1);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Dragon dragon = other.GetComponent<Dragon>();
        if (dragon != null)
        {
            dragon.TakeDamage(damage);
        }

        Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
