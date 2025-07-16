using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyPlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public LayerMask groundMask;
    public LayerMask wallMask;

    private Rigidbody rb;

    [Header("Movement")]
    public float moveSpeed = 7f;
    public float maxSpeed = 10f;
    public float jumpForce = 8f;
    public float airMultiplier = 0.4f;
    public float groundDrag = 5f;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public bool isGrounded;
    public float groundCheckRadius = 0.3f;
    public Transform groundCheck;

    [Header("Wall Running")]
    public float wallRunForce = 20f;
    public float wallJumpUpForce = 8f;
    public float wallJumpSideForce = 5f;
    public float wallCheckDistance = 0.8f;
    public float wallTiltAngle = 15f;
    public float wallRunDuration = 1.5f;

    private bool isWallRunning;
    private bool wallOnRight;
    private bool wallOnLeft;
    private float wallRunTimer;

    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        MyInput();
        GroundCheck();
        WallCheck();
        HandleDrag();
        HandleRotation();

        if (Input.GetKeyDown(KeyCode.Space) && isWallRunning)
        {
            WallJump();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();

        if (isWallRunning)
        {
            rb.AddForce(transform.up * wallRunForce, ForceMode.Force);
        }
    }

    void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
    }

    void HandleDrag()
    {
        rb.linearDamping = isGrounded ? groundDrag : 0f;
    }

    void MovePlayer()
    {
        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        LimitSpeed();
    }

    void LimitSpeed()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void WallCheck()
    {
        wallOnRight = Physics.Raycast(transform.position, orientation.right, wallCheckDistance, wallMask);
        wallOnLeft = Physics.Raycast(transform.position, -orientation.right, wallCheckDistance, wallMask);

        if ((wallOnRight || wallOnLeft) && !isGrounded && verticalInput > 0)
        {
            StartWallRun();
        }
        else
        {
            StopWallRun();
        }
    }

    void StartWallRun()
    {
        isWallRunning = true;
        wallRunTimer += Time.deltaTime;

        if (wallRunTimer > wallRunDuration)
        {
            StopWallRun();
        }

        rb.useGravity = false;

        // Tilt effect
        float targetZRotation = wallOnRight ? -wallTiltAngle : wallTiltAngle;
        Quaternion targetTilt = Quaternion.Euler(0, transform.eulerAngles.y, targetZRotation);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetTilt, Time.deltaTime * 5f);
    }

    void StopWallRun()
    {
        isWallRunning = false;
        wallRunTimer = 0f;
        rb.useGravity = true;

        Quaternion upright = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, upright, Time.deltaTime * 5f);
    }

    void WallJump()
    {
        Vector3 wallNormal = wallOnRight ? orientation.right : -orientation.right;
        Vector3 force = wallNormal * wallJumpSideForce + Vector3.up * wallJumpUpForce;

        rb.linearVelocity = new Vector3(0f, 0f, 0f);
        rb.AddForce(force, ForceMode.Impulse);
        StopWallRun();
    }

    void HandleRotation()
    {
        // Optional: make the body rotate to match movement direction
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
