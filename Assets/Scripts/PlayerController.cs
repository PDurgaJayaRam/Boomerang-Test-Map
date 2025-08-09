using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 12f; // Increased for smoother rotation
    public float accelerationTime = 0.2f; // Time to reach full speed
    public float decelerationTime = 0.3f; // Time to stop
    
    [Header("Jump")]
    public float jumpHeight = 1.0f; // Height to jump (2.5 - 1.5 = 1.0)
    private float jumpForce;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Mobile Controls")]
    public VirtualJoystick virtualJoystick;
    public JumpButton jumpButton;
    
    private Rigidbody rb;
    private bool isGrounded;
    private bool jumpRequested = false;
    
    // For smooth movement
    private Vector3 currentMovementVelocity;
    private Vector3 targetMovement;
    private float currentVelocityX;
    private float currentVelocityZ;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        
        // Calculate the exact jump force needed for the desired height
        jumpForce = Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * jumpHeight);
        
        // Try to find mobile controls if not set
        if (virtualJoystick == null)
        {
            virtualJoystick = FindObjectOfType<VirtualJoystick>();
        }
        
        if (jumpButton == null)
        {
            jumpButton = FindObjectOfType<JumpButton>();
        }
    }
    
    void Update()
    {
        // Get input from mobile controls or keyboard
        float horizontal = 0f;
        float vertical = 0f;
        bool jump = false;
        
        // Check if we have mobile controls
        bool useMobileControls = (virtualJoystick != null && jumpButton != null);
        
        if (useMobileControls)
        {
            // Use mobile controls
            Vector2 joystickInput = virtualJoystick.InputVector;
            horizontal = joystickInput.x;
            vertical = joystickInput.y;
            jump = jumpButton.IsPressed;
        }
        else
        {
            // Use keyboard controls
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            jump = Input.GetKeyDown(KeyCode.Space);
        }
        
        // Calculate movement direction in isometric space
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
        
        // Set target movement
        targetMovement = moveDirection * moveSpeed;
        
        // Smoothly accelerate/decelerate
        if (moveDirection != Vector3.zero)
        {
            // Accelerate
            currentMovementVelocity.x = Mathf.SmoothDamp(
                currentMovementVelocity.x, 
                targetMovement.x, 
                ref currentVelocityX, 
                accelerationTime
            );
            
            currentMovementVelocity.z = Mathf.SmoothDamp(
                currentMovementVelocity.z, 
                targetMovement.z, 
                ref currentVelocityZ, 
                accelerationTime
            );
        }
        else
        {
            // Decelerate
            currentMovementVelocity.x = Mathf.SmoothDamp(
                currentMovementVelocity.x, 
                0, 
                ref currentVelocityX, 
                decelerationTime
            );
            
            currentMovementVelocity.z = Mathf.SmoothDamp(
                currentMovementVelocity.z, 
                0, 
                ref currentVelocityZ, 
                decelerationTime
            );
        }
        
        // Apply movement while preserving vertical velocity
        Vector3 movement = new Vector3(currentMovementVelocity.x, rb.linearVelocity.y, currentMovementVelocity.z);
        rb.linearVelocity = movement;
        
        // Handle rotation
        if (moveDirection != Vector3.zero)
        {
            // Calculate target rotation based on movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            
            // Smoothly rotate towards target rotation
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
        
        // Handle jump input
        if (jump && isGrounded)
        {
            jumpRequested = true;
        }
    }
    
    void FixedUpdate()
    {
        // 3D ground check using sphere overlap
        Collider[] hitColliders = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundLayer);
        isGrounded = hitColliders.Length > 0;
        
        // Execute jump in FixedUpdate for physics accuracy
        if (jumpRequested && isGrounded)
        {
            // Set vertical velocity directly for precise jump height
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            jumpRequested = false;
        }
    }
    
    // Visualize ground check in editor
    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}