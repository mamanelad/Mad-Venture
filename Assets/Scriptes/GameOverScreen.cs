using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;


public class GameOverScreen : MonoBehaviour
{
    public float waitUntilShow = 1f;
    public void SetUp()
    {
        Invoke(nameof(ShowScreen), waitUntilShow);
    }

    private void ShowScreen()
    {
        gameObject.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene("level1");
    }
    
    public void MainMenue()
    {
        SceneManager.LoadScene("MainMenue");

    }
}
