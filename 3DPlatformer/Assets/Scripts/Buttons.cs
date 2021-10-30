using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    [SerializeField] PlayerController playerVariables;
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
        Debug.Log("Why no work?");
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
    private IEnumerator FadeToWhite(UnityEngine.UI.Image fadePanel) 
    {
        currentlyChanging = true;
       Color tmpAlpha = fadePanel.color;
        float tmpCount = 0.0f;
        while (tmpCount < playerVariables.timeToWaitForIENumberator)
        {
            yield return new WaitForSeconds(1.0f);
            tmpCount += Time.deltaTime;
            tmpAlpha.a += 0.1f;
            fadePanel.color = tmpAlpha;
        }
    }
    private void FixedUpdate()
    {
        if (alphaChanging && !currentlyChanging) 
        {
            StartCoroutine("FadeToWhite", playerVariables.fadeToWhitePanel);
        }
    }
}
