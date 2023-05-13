using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public MovementController movementController;

    private Command moveUpCommand, rotateLeftCommand, rotateRightCommand, stopThrustCommand, stopRotationCommand;

    [SerializeField] AudioClip mainEngine;

    [SerializeField] float movementSpeed = 1000f;
    [SerializeField] float rotationSpeed = 1f;

    [SerializeField] ParticleSystem rocketBoostParticles;
    [SerializeField] ParticleSystem leftThrustParticles;
    [SerializeField] ParticleSystem rightThrustParticles;

    private Invoker invoker;

    private void Awake()
    {
        invoker = new Invoker();

        movementController = FindObjectOfType<MovementController>();

        this.moveUpCommand = new MoveUp(movementSpeed, mainEngine, rocketBoostParticles);
        this.rotateLeftCommand = new RotateLeft(rotationSpeed, leftThrustParticles);
        this.rotateRightCommand = new RotateRight(rotationSpeed, rightThrustParticles);
        this.stopThrustCommand = new StopThrust(mainEngine, rocketBoostParticles);
        this.stopRotationCommand = new StopRotation(leftThrustParticles, rightThrustParticles);
    }


    private void Update()
    {

        var gameManager = FindObjectOfType<GameManager>();
        if (gameManager.LevelIsOver)
        {
            this.enabled = false;
            return;
        }


        if (movementController == null)
        {
            Debug.LogError("MovementController is not assigned in the InputHandler script.");
            return;
        }
        if (Input.GetKey(KeyCode.Space) || CrossPlatformInputManager.GetButton("Thrust"))
        {
            invoker.ExecuteCommand(moveUpCommand, movementController.Rigidbdy, movementController.AS);
        }
        if (Input.GetKeyUp(KeyCode.Space) || CrossPlatformInputManager.GetButtonUp("Thrust"))
        {
            invoker.ExecuteCommand(stopThrustCommand, movementController.Rigidbdy, movementController.AS);
        }

        if (Input.GetKey(KeyCode.A) || CrossPlatformInputManager.GetButton("Left"))
        {
            invoker.ExecuteCommand(rotateLeftCommand, movementController.Rigidbdy, movementController.AS);
        }
        if (Input.GetKey(KeyCode.D) || CrossPlatformInputManager.GetButton("Right"))
        {
            invoker.ExecuteCommand(rotateRightCommand, movementController.Rigidbdy, movementController.AS);
        }
        if (Input.GetKeyUp(KeyCode.D) || CrossPlatformInputManager.GetButtonUp("Right") || Input.GetKeyUp(KeyCode.A) || CrossPlatformInputManager.GetButtonUp("Left"))
        {
            invoker.ExecuteCommand(stopRotationCommand, movementController.Rigidbdy, movementController.AS);
        }
    }
}
