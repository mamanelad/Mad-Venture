using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordBonuse : MonoBehaviour
{
    private string _name;

    private void Awake()
    {
        _name = gameObject.name;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<Player>();
        player.BuildSwordShooter();
        //TODO : call some animation and when animation is finise. 
        Destroy(gameObject);

        
    }
}
