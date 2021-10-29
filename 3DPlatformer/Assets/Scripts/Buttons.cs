using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    [SerializeField] PlayerController playerVariables;
    [SerializeField] UnityEngine.UI.Image fadeToWhitePanel;
    bool alphaChanging=false;
    bool currentlyChanging = false;
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
    public void StartFading() 
    {
        alphaChanging = true;
    }
    private IEnumerator FadeToWhite() 
    {
        currentlyChanging = true;
       Color tmpAlpha = fadeToWhitePanel.color;
        float tmpCount = 0.0f;
        while (tmpCount < playerVariables.timeToWaitForIENumberator)
        {
            yield return new WaitForSeconds(0.5f);
            tmpCount += 0.5f;
            tmpAlpha.a += 10f;
            fadeToWhitePanel.color = tmpAlpha;
        }
    }
    private void FixedUpdate()
    {
        if (alphaChanging && !currentlyChanging) 
        {

            StartCoroutine("FadeToWhite");
        }
    }
}
