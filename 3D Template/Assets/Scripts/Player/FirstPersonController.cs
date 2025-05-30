using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpForce = 5f;
    private float gravity = -9.81f;

    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float lookXLimit = 80f;
    private bool invertY = false;

    [Header("References")]
    [SerializeField] private Camera playerCamera;

    private CharacterController characterController;
    private float rotationX;
    private Vector3 velocity;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // If no camera is assigned, try to find one in children
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
            {
                Debug.LogError("No camera assigned to FirstPersonController! Please assign a camera in the inspector.");
            }
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleMouseLook();
    }

    private void HandleMovement()
    {
        // Use CharacterController's built-in isGrounded check
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Get input axes
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Apply movement speed (sprint if shift is held)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        characterController.Move(move * currentSpeed * Time.deltaTime);

        // Handle jumping
        if (Input.GetButtonDown("Jump") && characterController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Calculate camera rotation
        rotationX += mouseY * (invertY ? 1 : -1);
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        // Apply rotations
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, mouseX, 0);
    }

    // Optional: Method to toggle cursor lock
    public void ToggleCursorLock()
    {
        Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = !Cursor.visible;
    }
} 