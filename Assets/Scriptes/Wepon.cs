using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wepon : MonoBehaviour
{

    public Transform FirePoint;

    public GameObject swordPrefab;
    // Start is called before the first frame update

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
        
    }

    private void Shoot()
    {
        var x = Instantiate(swordPrefab, FirePoint.position, FirePoint.rotation);
        print(x.name);
    }
    
}
