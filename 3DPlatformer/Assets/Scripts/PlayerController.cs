using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    // Terry's motor and other important objects so we can use them
    [Header("Assignables")]
    public KinematicPlayerMotor motor;
    public PlayerInput input;
    public GameObject model;
    public GameObject bomb;
    public ParticleSystem bombGoBoom;
    [SerializeField] Buttons UiControl;
    public Animator motorAnimations;
    public Animator UIAnimation;
    //important variables not in motor
    [Header("")]
    public float interactRadius = 5.0f;
    public bool pause = false;
    public bool gameEnded = false;
    public bool gameWon = false;
    private int currentLevel;
    private WaitForSeconds delay = null;
    [Range(1, 100)]
    public float timeToWaitForIENumberator = 10.0f;
    public UnityEngine.UI.Image fadeToWhitePanel;
    // Start is called before the first frame update
    void OnEnable()
    {
        input.currentActionMap["Jumping"].performed += HandleJumpPlayer;
        input.currentActionMap["Special"].performed += HandleTriggerSpecial; //this will most likely be some kind of roll or jump
        input.currentActionMap["Dash"].performed += HandleSonicLevelsOfDash;
        input.currentActionMap["Pause"].performed += HandlePauseTheGame;
    }
    private void Start()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainScene")) //this will check the current scene so that the CurrentLevel will always be correct.
        {
            currentLevel = 1; //this will always be level 1 but the build index might not stay as 1
            PlayerPrefs.SetInt("CurrentLevel", 1);
        }
        else
        {
            currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        }
        delay = new WaitForSeconds(timeToWaitForIENumberator); //if I need to change this I will change the variable down the line
    }
    private void OnDisable()
    {
        try
        {
            input.currentActionMap["Jumping"].performed -= HandleJumpPlayer;
            input.currentActionMap["Special"].performed -= HandleTriggerSpecial;
            input.currentActionMap["Dash"].performed -= HandleSonicLevelsOfDash;
            input.currentActionMap["Pause"].performed -= HandlePauseTheGame;
        }
        catch (NullReferenceException)
        {
            Debug.LogWarning("Failed to unsubscribe from the input event methods due to the input manager being removed first", this);
        }
    }
    private void HandleTriggerSpecial(InputAction.CallbackContext obj)
    {
        if (!gameEnded)
        {
            Vector3 distanceVec = bomb.transform.position - transform.position;
            if (distanceVec.sqrMagnitude <= interactRadius * interactRadius)
            {
                EndingTheGame(false);
            }
        }
    }
    private void HandleJumpPlayer(InputAction.CallbackContext obj) //this will be called when the player hits the space bar or whatever binding I give to jump and simply apply the jump vector in terry's code
    {
        if (!gameEnded)
        {
            motor.JumpInput();
        }
    }
    private void HandleSonicLevelsOfDash(InputAction.CallbackContext obj)
    {
        if (!gameEnded)
        {
            motor.DashInput();
        }
    }
    private void HandlePauseTheGame(InputAction.CallbackContext obj)
    {
        if (!gameEnded)
        {
            pause = !pause;
            UIAnimation.SetBool("Paused", pause);
        }
    }
    public void PauseTheGame()
    {
        if (!gameEnded)
        {
            pause = !pause;
            UIAnimation.SetBool("Paused", pause);
        }
    }
    public void EndingTheGame(bool playerLoss)
    {
        if (playerLoss)
        {
            PlayerPrefs.SetString("Level" + currentLevel,"In level " + currentLevel + " Bomb went boom"); //this will save the current level for displaying purposes on the credits
            gameEnded = true;
            bombGoBoom.Play();
            UiControl.StartFading();
            Invoke("WaitForTime", timeToWaitForIENumberator);
        }
        else
        {
            PlayerPrefs.SetString("Level" + currentLevel,"Level " + currentLevel + " completed with " + (motor.bombTime - motor.currentTime).ToString("00.00") + " time left."); // this only can be grabbed if bomb time is greater then currentTime
            PlayerPrefs.SetInt("CurrentLevel", ++currentLevel);
            gameEnded = true;
            gameWon = true;
            Invoke("MoveToNextLevel", timeToWaitForIENumberator/2);
        }
    }
    public void ApplyDashReset()
    {

        motor.dashDelay = 0; //removing the cooldown
    }
    private void WaitForTime()
    {
        SceneManager.LoadScene("GameOverScreen"); //this is technically for both winning and losing but you'll get here faster by losing!
    }
    private void MoveToNextLevel() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //we'll increment up until we make it to the gameOverScreen
    }
    private void Update()//we'll use update to handle our actual movement/animation
    {
        if (!gameEnded)
        {
            Vector2 movementVec2 = input.currentActionMap["Walking"].ReadValue<Vector2>();
            motor.MoveInput(new Vector3(movementVec2.x, 0.0f, movementVec2.y));
            if (movementVec2.magnitude > 0)
            {
                model.transform.forward = new Vector3(movementVec2.x, 0, movementVec2.y);
                motorAnimations.SetBool("isMoving", true);
            }
            else
            {
                motorAnimations.SetBool("isMoving", false);
            }
        }

        //jumping animation
        motorAnimations.SetBool("isJumping", motor.Grounded);
    }

}
