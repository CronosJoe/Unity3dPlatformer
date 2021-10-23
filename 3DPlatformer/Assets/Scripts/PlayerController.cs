using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Terry's motor so we can use it
    public KinematicPlayerMotor motor;
    public PlayerInput input;
    public GameObject model;
    public GameObject bomb;
    //important variables not in motor
    public float interactRadius = 5.0f;
    public bool pause;
    //implement animations here or in another script
    public Animator allThings;
    public Animator UIAnimation;
    // Start is called before the first frame update
    void OnEnable()
    {
        input.currentActionMap["Jumping"].performed += JumpPlayer;
        input.currentActionMap["Special"].performed += TriggerSpecial; //this will most likely be some kind of roll or jump
        input.currentActionMap["Dash"].performed += SonicLevelsOfDash;
        input.currentActionMap["Pause"].performed += PauseTheGame;
    }

    private void OnDisable()
    {
        try
        {
            input.currentActionMap["Jumping"].performed -= JumpPlayer;
        }
        catch(NullReferenceException)
        {
            Debug.Log("Failed to unsubscribe from the jumpPlayer method due to the input manager being removed first");
        }
        try
        {

            input.currentActionMap["Special"].performed -= TriggerSpecial;
        }
        catch(NullReferenceException)
        {
            Debug.Log("Failed to unsubscribe from the TriggerSpecial method due to the input manager being removed first");
        }
        try
        {

            input.currentActionMap["Dash"].performed -= SonicLevelsOfDash;
        }
        catch(NullReferenceException)
        {
            Debug.Log("Failed to unsubscribe from the SonicLevelsOfDash method due to the input manager being removed first");
        }
        try
        {

            input.currentActionMap["Pause"].performed -= PauseTheGame;
        }
        catch(NullReferenceException)
        {
            Debug.Log("Failed to unsubscribe from the PauseTheGame method due to the input manager being removed first");
        }
        
    }
    private void TriggerSpecial(InputAction.CallbackContext obj)
    {
        Vector3 distanceVec = bomb.transform.position - transform.position;
        Debug.Log(distanceVec.magnitude);
        if (distanceVec.magnitude <= interactRadius) 
        {
            Debug.Log("test");
            //take the input and start/disarm the bomb
            //for now calling game win so that this method works
            EndingTheGame(false);
        }
    }
    private void JumpPlayer(InputAction.CallbackContext obj) //this will be called when the player hits the space bar or whatever binding I give to jump and simply apply the jump vector in terry's code
    {
        motor.JumpInput();
    }
    private void SonicLevelsOfDash(InputAction.CallbackContext obj)
    {
        motor.DashInput();
    }
    private void PauseTheGame(InputAction.CallbackContext obj)
    {
        pause = !pause;
        UIAnimation.SetBool("Paused", pause);
    }
    public void PauseTheGame() 
    {
        pause = !pause;
        UIAnimation.SetBool("Paused", pause);
    }

    public void EndingTheGame(bool playerLoss) 
    {
        if (playerLoss) 
        {
            SceneManager.LoadScene("GameOverScreen"); //this is technically for both winning and losing but you'll get here faster by losing!
        }
        else 
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //we'll increment up until we make it to the gameOverScreen
        }
    }
    public void ApplyDashReset() 
    {
        
        motor.dashDelay = 0; //removing the cooldown
    }
    private void Update()//we'll use update to handle our actual movement/animation
    {
        Vector2 movementVec2 = input.currentActionMap["Walking"].ReadValue<Vector2>();
        motor.MoveInput(new Vector3(movementVec2.x, 0.0f, movementVec2.y));
        if (movementVec2.magnitude > 0) 
        {
            model.transform.forward = new Vector3(movementVec2.x, 0, movementVec2.y);
            allThings.SetBool("isMoving", true);
        }
        else 
        {
            allThings.SetBool("isMoving", false);
        }
        //jumping animation
        allThings.SetBool("isJumping", motor.Grounded);
    }

}
