using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")] [SerializeField]
    private float moveSpeed = 4f;

    [SerializeField] private float sprintSpeedMultiplier = 1.5f;
    [SerializeField] private float sprintTransitionSpeed = 10f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float jumpHeight = 0.6f;
    [SerializeField] private bool allowDoubleJump = true;

    [Header("Input Actions")] public InputActionReference moveAction;

    public InputActionReference jumpAction;
    public InputActionReference sprintAction;
    private bool canDoubleJump;

    [Header("References")] private CharacterController controller;

    private float currentSpeed;
    private bool isSprinting;
    private bool jumpRequested;

    private Vector2 moveInput = Vector2.zero;

    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (moveAction != null)
            moveInput = moveAction.action.ReadValue<Vector2>();
        else
            moveInput = Vector2.zero;

        HandleMovement();
    }

    private void OnEnable()
    {
        moveAction?.action.Enable();
        jumpAction?.action.Enable();
        sprintAction?.action.Enable();

        // Callbacks
        if (jumpAction != null) jumpAction.action.performed += OnJumpPerformed;

        if (sprintAction != null)
        {
            sprintAction.action.performed += ctx => isSprinting = true;
            sprintAction.action.canceled += ctx => isSprinting = false;
        }
    }

    private void OnDisable()
    {
        if (jumpAction != null) jumpAction.action.performed -= OnJumpPerformed;

        moveAction?.action.Disable();
        jumpAction?.action.Disable();
        sprintAction?.action.Disable();
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        jumpRequested = true;
    }

    private void HandleMovement()
    {
        // On determine le movement horizontal :
        var forward = transform.forward;
        var right = transform.right;
        var horizontal = forward * moveInput.y + right * moveInput.x;
        horizontal = horizontal.normalized; // Pour le mouvement en diagonale 

        // Transition de sprint :
        var targetMultiplier = isSprinting ? sprintSpeedMultiplier : 1f;
        var targetSpeed = moveSpeed * targetMultiplier;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, sprintTransitionSpeed * Time.deltaTime);

        // Vertical physics
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f) verticalVelocity = -2f;
            // Saut :
            if (jumpRequested)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
                canDoubleJump = allowDoubleJump;
                jumpRequested = false;
            }
        }
        else
        {
            // DOuble saut : 
            if (jumpRequested && canDoubleJump)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
                canDoubleJump = false;
                jumpRequested = false;
            }

            // apply gravity
            verticalVelocity -= gravity * Time.deltaTime;
        }

        var velocity = horizontal * currentSpeed;
        velocity.y = verticalVelocity;

        // Move CharacterController
        controller.Move(velocity * Time.deltaTime);
    }

    public void SetAllowDoubleJump(bool allow)
    {
        allowDoubleJump = allow;
    }
}