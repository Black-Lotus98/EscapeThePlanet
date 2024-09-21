using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MovementController movementController;
    
    [Header("Audio")]
    [SerializeField] private AudioClip mainEngine;
    [SerializeField] private AudioClip shieldActivationSound;
    
    [Header("Settings")]
    [SerializeField] private float movementSpeed = 1000f;
    [SerializeField] private float rotationSpeed = 1f;
    
    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem rocketBoostParticles;
    [SerializeField] private ParticleSystem leftThrustParticles;
    [SerializeField] private ParticleSystem rightThrustParticles;

    // Cached components
    private Invoker invoker;
    private FuelManager fuelManager;
    private ShieldManager shieldManager;
    private GameManager gameManager;
    private TutorialManager tutorialManager;
    
    // Commands
    private Command moveUpCommand;
    private Command rotateLeftCommand;
    private Command rotateRightCommand;
    private Command stopThrustCommand;
    private Command stopRotationCommand;
    private Command toggleShieldCommand;

    private void Awake()
    {
        // Cache components once
        invoker = new Invoker();
        fuelManager = FindObjectOfType<FuelManager>();
        shieldManager = FindObjectOfType<ShieldManager>();
        gameManager = FindObjectOfType<GameManager>();
        tutorialManager = FindObjectOfType<TutorialManager>();
        
        // Validate required components
        if (movementController == null)
        {
            Debug.LogError("MovementController is not assigned in the InputHandler script.");
            enabled = false;
            return;
        }

        // Initialize commands
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        moveUpCommand = new MoveUp(movementSpeed, mainEngine, rocketBoostParticles, fuelManager);
        rotateLeftCommand = new RotateLeft(rotationSpeed, leftThrustParticles);
        rotateRightCommand = new RotateRight(rotationSpeed, rightThrustParticles);
        stopThrustCommand = new StopThrust(mainEngine, rocketBoostParticles);
        stopRotationCommand = new StopRotation(leftThrustParticles, rightThrustParticles);
        toggleShieldCommand = new ToggleShield(shieldManager, shieldActivationSound);
    }

    private void Update()
    {
        // Early return if tutorial is active or level is over
        if (tutorialManager == null && gameManager != null && gameManager.LevelIsOver)
        {
            enabled = false;
            return;
        }

        // Handle input with early returns for better performance
        HandleThrustInput();
        HandleRotationInput();
        HandleShieldInput();
    }

    private void HandleThrustInput()
    {
        bool isThrusting = Input.GetKey(KeyCode.Space) || CrossPlatformInputManager.GetButton("Thrust");
        bool thrustReleased = Input.GetKeyUp(KeyCode.Space) || CrossPlatformInputManager.GetButtonUp("Thrust");
        
        if (isThrusting)
        {
            invoker.ExecuteCommand(moveUpCommand, movementController.Rigidbdy, movementController.AS);
        }
        else if (thrustReleased)
        {
            invoker.ExecuteCommand(stopThrustCommand, movementController.Rigidbdy, movementController.AS);
        }
    }

    private void HandleRotationInput()
    {
        bool isRotatingLeft = Input.GetKey(KeyCode.A) || CrossPlatformInputManager.GetButton("Left");
        bool isRotatingRight = Input.GetKey(KeyCode.D) || CrossPlatformInputManager.GetButton("Right");
        bool rotationReleased = Input.GetKeyUp(KeyCode.D) || CrossPlatformInputManager.GetButtonUp("Right") || 
                               Input.GetKeyUp(KeyCode.A) || CrossPlatformInputManager.GetButtonUp("Left");
        
        if (isRotatingLeft)
        {
            invoker.ExecuteCommand(rotateLeftCommand, movementController.Rigidbdy, movementController.AS);
        }
        else if (isRotatingRight)
        {
            invoker.ExecuteCommand(rotateRightCommand, movementController.Rigidbdy, movementController.AS);
        }
        else if (rotationReleased)
        {
            invoker.ExecuteCommand(stopRotationCommand, movementController.Rigidbdy, movementController.AS);
        }
    }

    private void HandleShieldInput()
    {
        bool shieldPressed = Input.GetKeyDown(KeyCode.E) || CrossPlatformInputManager.GetButtonDown("Shield");
        
        if (shieldPressed)
        {
            invoker.ExecuteCommand(toggleShieldCommand, movementController.Rigidbdy, movementController.AS);
        }
    }
}
