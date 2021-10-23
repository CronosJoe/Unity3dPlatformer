using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    [SerializeField] PlayerController playerVariables;
    public void OnQuitButton() 
    {
        //do any clean up needed here
        Application.Quit();
    }
    public void OnPlayButton() 
    {
        SceneManager.LoadScene("MainScene");
    }
    public void OnSettingsButton() 
    {
    //load up whatever settings I want
    }
    public void OnContinue() 
    {
        playerVariables.PauseTheGame();
    }
    public void ToMainMenu() 
    {
        SceneManager.LoadScene("MainMenu");
    }
}
