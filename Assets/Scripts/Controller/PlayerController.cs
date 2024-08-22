using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the movement of the player with given input from the input manager
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The speed  at which the player moves")]
    public float moveSpeed = 2f;
    [Tooltip("The speed  at the player rotates to look left and right (calculated in degrees)")]
    public float lookSpeed = 60f;
    [Tooltip("The ´power which the player jumps")]
    public float jumpPower = 8f;
    [Tooltip("The strengh of gravity")]
    public float gravity = 9.81f;
    [Header("jump timing")]
    public float jumpTimeLeniency = 0.1f;
    float timeToStopBeingLenient = 0;
    
    [Header("Required References")]
    [Tooltip("The player shooter script that fires projectiles")]
    public Shooter playerShooter;
    public Health playerHealth;
    public List<GameObject> disableWhileDead;
    bool doubleJumpAvailable = false;

    private CharacterController controller;
    private InputManager inputM;
    /// <summary>
    /// Description:
    /// Standard Unity function called once before the first Update call
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    void Start()
    {
        SetUpCharacterController();
        SetUpInputManager();
    }
    
    private void SetUpCharacterController()
    {
        controller = GetComponent<CharacterController>();

        if(controller == null)
        {
            Debug.LogError("error controller == null");
        }
    }
    private void SetUpInputManager()
    {
        inputM = InputManager.instance;
    }

    /// <summary>
    /// Description:
    /// Standard Unity function called once every frame
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    void Update()
    {
        if (playerHealth.currentHealth <= 0)
        {
            foreach (GameObject inGameObject in disableWhileDead)
            {
                inGameObject.SetActive(false);

            }
            Debug.Log("teste");
            return;
        }
        else
        {
            foreach (GameObject inGameObject in disableWhileDead)
            {
                inGameObject.SetActive(true);

            }
           
        }
        
       

        ProcessMovement();
        ProcessRotation();

    }
    Vector3 moveDirection;

    void ProcessMovement()
    {
        //get the inputs from input manager
        float leftRightInput = inputM.horizontalMoveAxis;
        float fowardBackwardInput = inputM.verticalMoveAxis;
        bool jumpPressed = inputM.jumpPressed;

        //controla o player enquanto está no chão
        if (controller.isGrounded)
        {
            doubleJumpAvailable = true;
            timeToStopBeingLenient = Time.time + jumpTimeLeniency;
            // seta a direção do movimento a ser recebida pelo input, seta y = 0 desde que esteja no chão
            moveDirection = new Vector3(leftRightInput, 0, fowardBackwardInput);
            // seta o movedirection em relação ao transform
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection = moveDirection * moveSpeed;
            if (jumpPressed)
            {
                moveDirection.y = jumpPower;
            }
        }
        else
        {
            moveDirection = new Vector3(leftRightInput * moveSpeed, moveDirection.y, fowardBackwardInput * moveSpeed);
            moveDirection = transform.TransformDirection(moveDirection);
            
            if(jumpPressed && Time.time < timeToStopBeingLenient)
            {
                moveDirection.y = jumpPower;
            }
            else if(jumpPressed && doubleJumpAvailable)
            {
                moveDirection.y = jumpPower;
                doubleJumpAvailable = false;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        if(controller.isGrounded && moveDirection.y < 0)
        {
            moveDirection.y = -0.3f;
        }

        controller.Move(moveDirection * Time.deltaTime);
        
    }
    void ProcessRotation()
    {
        float horizontalLookInput = inputM.horizontalLookAxis;
        Vector3 playerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(new Vector3(playerRotation.x, playerRotation.y + horizontalLookInput * lookSpeed * Time.deltaTime,
            playerRotation.z));
    }
}
