using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("level1");
    }

    public void ExitGame()
    {
        Application.Quit();
        // UnityEditor.EditorApplication.isPlaying = false;
    }
}