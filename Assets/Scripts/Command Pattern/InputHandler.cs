using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    // New Input System (generated wrapper)
    private PlayerMovement controls;
    private bool wasThrusting;
    private bool wasRotating;

    private void Awake()
    {
        // Cache components once
        invoker = new Invoker();
        controls = new PlayerMovement();
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

    private void OnEnable()
    {
        controls?.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls?.Gameplay.Disable();
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
        bool isThrusting = controls.Gameplay.Thrust.IsPressed();

        if (isThrusting)
        {
            invoker.ExecuteCommand(moveUpCommand, movementController.Rigidbdy, movementController.AS);
        }
        else if (wasThrusting)
        {
            invoker.ExecuteCommand(stopThrustCommand, movementController.Rigidbdy, movementController.AS);
        }

        wasThrusting = isThrusting;
    }

    private void HandleRotationInput()
    {
        // Rotate axis: negative = left (A / LeftArrow), positive = right (D / RightArrow)
        float rotate = controls.Gameplay.Rotate.ReadValue<float>();

        if (rotate < 0f)
        {
            invoker.ExecuteCommand(rotateLeftCommand, movementController.Rigidbdy, movementController.AS);
            wasRotating = true;
        }
        else if (rotate > 0f)
        {
            invoker.ExecuteCommand(rotateRightCommand, movementController.Rigidbdy, movementController.AS);
            wasRotating = true;
        }
        else if (wasRotating)
        {
            invoker.ExecuteCommand(stopRotationCommand, movementController.Rigidbdy, movementController.AS);
            wasRotating = false;
        }
    }

    private void HandleShieldInput()
    {
        if (controls.Gameplay.Shield.WasPressedThisFrame())
        {
            invoker.ExecuteCommand(toggleShieldCommand, movementController.Rigidbdy, movementController.AS);
        }
    }
}
