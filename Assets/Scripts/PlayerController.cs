using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    
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
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        
        // Calculate the exact jump force needed for the desired height
        jumpForce = Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * jumpHeight);
    }
    
    void Update()
    {
        // Get input from mobile controls or keyboard
        float horizontal = 0f;
        float vertical = 0f;
        bool jump = false;
        
        // Check if we're on a mobile platform and have mobile controls
        bool useMobileControls = false;
        
        #if UNITY_ANDROID || UNITY_IOS
        useMobileControls = (virtualJoystick != null && jumpButton != null);
        #endif
        
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
        
        // Apply movement while preserving vertical velocity
        Vector3 movement = moveDirection * moveSpeed;
        movement.y = rb.linearVelocity.y; // Preserve gravity/jump velocity
        rb.linearVelocity = movement;
        
        // Rotate player to face movement direction
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