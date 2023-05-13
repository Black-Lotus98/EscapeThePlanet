using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class MovementController : MonoBehaviour
{



    public Rigidbody Rigidbdy { get; private set; }
    public AudioSource AS { get; private set; }


    Player player;

    private void Start()
    {
        Rigidbdy = GetComponent<Rigidbody>();
        AS = GetComponent<AudioSource>();
    }


    // private void Update()
    // {
    //     var gameManager = FindObjectOfType<GameManager>();

    //     if (gameManager.LevelIsOver)
    //     {
    //         this.enabled = false;
    //         return;
    //     }
    //     if (Input.GetKey(KeyCode.Space) || CrossPlatformInputManager.GetButton("Thrust"))
    //     {
    //         invoker.ExecuteCommand(MoveUpCommand, Rigidbdy, AS);
    //     }
    //     else
    //     {
    //         invoker.ExecuteCommand(StopThrustCommand, Rigidbdy, AS);
    //     }

    //     if (Input.GetKey(KeyCode.A) || CrossPlatformInputManager.GetButton("Left"))
    //     {
    //         invoker.ExecuteCommand(RotateLeftCommand, Rigidbdy, AS);
    //     }
    //     else if (Input.GetKey(KeyCode.D) || CrossPlatformInputManager.GetButton("Right"))
    //     {
    //         invoker.ExecuteCommand(RotateRightCommand, Rigidbdy, AS);
    //     }
    //     else
    //     {
    //         invoker.ExecuteCommand(StopRotationCommand, Rigidbdy, AS);
    //     }
    // }




    // public void MoveUp()
    // {
    //     transform.Translate(Vector3.up * Movementspeed * Time.deltaTime);
    //     // rigidbdy.AddForce(Vector3.up * Movementspeed);
    // }

    // public void RotateRight()
    // {
    //     transform.Rotate(Vector3.up * Rotationspeed * Time.deltaTime);
    // }

    // public void RotateLeft()
    // {
    //     transform.Rotate(Vector3.up * -Rotationspeed * Time.deltaTime);
    // }



}