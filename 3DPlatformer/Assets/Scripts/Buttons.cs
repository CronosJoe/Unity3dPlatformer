using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    [SerializeField] PlayerController playerVariables;
    [SerializeField] TMP_Text timeScores;
    bool alphaChanging=false;
    bool currentlyChanging = false;
    private void Start() 
    {
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Credits")) 
        {
            timeScores.text = PlayerPrefs.GetString("Level1") + "\n" + PlayerPrefs.GetString("Level2");
        }
    }
    public void OnQuitButton() 
    {
        //do any clean up needed here
        Application.Quit();
    }
    public void OnPlayButton() 
    {
        SceneManager.LoadScene("MainScene");
    }
    public void OnCreditsButton() 
    {
        SceneManager.LoadScene("Credits");
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
