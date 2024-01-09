using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public MovementController movementController;

    private Command moveUpCommand, rotateLeftCommand, rotateRightCommand, stopThrustCommand, stopRotationCommand, toggleShieldCommand;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip shieldActivationSound;

    [SerializeField] float movementSpeed = 1000f;
    [SerializeField] float rotationSpeed = 1f;

    [SerializeField] ParticleSystem rocketBoostParticles;
    [SerializeField] ParticleSystem leftThrustParticles;
    [SerializeField] ParticleSystem rightThrustParticles;

    private Invoker invoker;

    private void Awake()
    {
        invoker = new Invoker();
        FuelManager fuelManager = FindObjectOfType<FuelManager>();
        ShieldManager shieldManager = FindObjectOfType<ShieldManager>();
        movementController = FindObjectOfType<MovementController>();

        this.moveUpCommand = new MoveUp(movementSpeed, mainEngine, rocketBoostParticles, fuelManager);
        this.rotateLeftCommand = new RotateLeft(rotationSpeed, leftThrustParticles);
        this.rotateRightCommand = new RotateRight(rotationSpeed, rightThrustParticles);
        this.stopThrustCommand = new StopThrust(mainEngine, rocketBoostParticles);
        this.stopRotationCommand = new StopRotation(leftThrustParticles, rightThrustParticles);
        this.toggleShieldCommand = new ToggleShield(shieldManager, shieldActivationSound);
    }


    private void Update()
    {
        // This if statement is for the tutorial levels, as it fix the issue where the controller gets stuck and stop functioning
        if (FindObjectOfType<TutorialManager>() == null)
        {
            var gameManager = FindObjectOfType<GameManager>();
            if (gameManager.LevelIsOver)
            {
                this.enabled = false;
                return;
            }
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
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow) || CrossPlatformInputManager.GetButtonUp("Right") || Input.GetKeyUp(KeyCode.A) || CrossPlatformInputManager.GetButtonUp("Left"))
        {
            invoker.ExecuteCommand(stopRotationCommand, movementController.Rigidbdy, movementController.AS);
        }
        if (Input.GetKeyDown(KeyCode.E) || CrossPlatformInputManager.GetButtonDown("Shield"))
        {
            invoker.ExecuteCommand(toggleShieldCommand, movementController.Rigidbdy, movementController.AS);
        }
    }
}
