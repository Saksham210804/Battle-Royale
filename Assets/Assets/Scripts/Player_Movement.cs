using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player_Movement : MonoBehaviour
{
    public CharacterController CharacterController;
    public Transform Ground_Check;
    public Transform orientation;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float J_Height = 5f;
    public float groundDistance = 0.4f;
    public float wallcheckdistance = 0.8f;
    public float walljumpsideforce = 8f;
    public LayerMask groundMask;
    public LayerMask Wall_Run;
    public float Health = 100f;
    public float currentHealth;
    public Vector3 velocity;

    private bool IsGrounded;
    private bool isWallRunning = false;
    public bool isWallJumping = false;

    private float wallJumpTimer = 0f;
    private Vector3 wallJumpSideForce = Vector3.zero;
    public float wallJumpDuration = 0.2f;
    public float wallRunUpwardJumpForce = 6f;
    public Slider HealthBar;
    public bool InCover;
    private Coroutine healingCoroutine = null;
    public GameObject Defeated_UI;
    private void Start()
    {
        currentHealth = Health;
        HealthBar.maxValue = Health;
    }

    void Update()
    {
        // Ground Check
        IsGrounded = Physics.CheckSphere(Ground_Check.position, groundDistance, groundMask);
        if (IsGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (IsGrounded)
            isWallJumping = false;

        // Movement input
        float move_x = Input.GetAxis("Horizontal");
        float move_z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * move_x + transform.forward * move_z;

        // Apply movement
        CharacterController.Move(move * speed * Time.deltaTime);

        // Ground jump
        if (Input.GetButtonDown("Jump") && IsGrounded)
        {
            velocity.y = Mathf.Sqrt(J_Height * -2f * gravity);
        }

        // Wall run + wall jump
        HandleWallRun(move_z);

        // Gravity (only when not wall running or wall jumping)
        if (!isWallRunning && !isWallJumping)
            velocity.y += gravity * Time.deltaTime;

        // Apply temporary wall jump side force
        if (wallJumpTimer > 0)
        {
            wallJumpTimer -= Time.deltaTime;

            // Smoothly reduce side force over time
            wallJumpSideForce = Vector3.Lerp(wallJumpSideForce, Vector3.zero, Time.deltaTime * 5f);
            CharacterController.Move(wallJumpSideForce * Time.deltaTime);
        }
        else
        {
            isWallJumping = false; // reset wall jump after timer
        }

        // Final velocity apply
        CharacterController.Move(velocity * Time.deltaTime);
        if (InCover && currentHealth <= 50f && healingCoroutine == null)
        {
            healingCoroutine = StartCoroutine(Healing());
        }

        // Stop healing if player leaves cover
        if (!InCover && healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine);
            healingCoroutine = null;
        }


    }
    IEnumerator Healing()
    {
        float covertime = 0f;

        // Wait in cover for 5 seconds
        while (covertime < 5f)
        {
            // Exit if player leaves cover
            if (!InCover)
            {
                healingCoroutine = null;
                yield break;
            }

            covertime += Time.deltaTime;
            yield return null;
        }

        // Begin healing gradually
        while (InCover && currentHealth < 100f)
        {
            currentHealth += 10f * Time.deltaTime; // Heal 10 HP/sec
            currentHealth = Mathf.Clamp(currentHealth, 0f, 100f);

            HealthBar.value = currentHealth;

            yield return null;
        }

        healingCoroutine = null; // Mark coroutine as done
    }

    public void AddHealth( float extra_hp)
    {
        currentHealth += extra_hp;
        HealthBar.value = currentHealth;
    }
    void HandleWallRun(float moveZ)
    {
        isWallRunning = false;

        if (IsGrounded || moveZ <= 0)
            return;

        RaycastHit hitR, hitL;
        Vector3 origin = transform.position + Vector3.up * 1f;

        bool rightWall = Physics.Raycast(origin, orientation.right, out hitR, wallcheckdistance, Wall_Run);
        bool leftWall = Physics.Raycast(origin, -orientation.right, out hitL, wallcheckdistance, Wall_Run);

        Debug.DrawRay(origin, orientation.right * wallcheckdistance, rightWall ? Color.green : Color.red);
        Debug.DrawRay(origin, -orientation.right * wallcheckdistance, leftWall ? Color.green : Color.red);

        if (rightWall || leftWall)
        {
            isWallRunning = true;
            velocity.y = 0f;

            // Tilt player
            Quaternion tiltRotation = Quaternion.Euler(0f, transform.eulerAngles.y, rightWall ? -15f : 15f);
            transform.rotation = Quaternion.Slerp(transform.rotation, tiltRotation, Time.deltaTime * 5f);

            // Wall jump
            if (Input.GetButtonDown("Jump") )
            {
                isWallJumping = true;
                Vector3 wallNormal = rightWall ? hitR.normal : hitL.normal;
                float jumpUpForce = Mathf.Sqrt(wallRunUpwardJumpForce * -2f * gravity);

                velocity.y = jumpUpForce;
                wallJumpSideForce = wallNormal * walljumpsideforce;
                wallJumpTimer = wallJumpDuration;

                isWallRunning = false;
            }
        }
        else
        {
            // Reset rotation
            Quaternion upright = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, upright, Time.deltaTime * 5f);
        }
    }

    public void Player_Damage(float hurt)
    {
        InCover = false; // Reset cover state when taking damage
        currentHealth -= hurt;
        currentHealth = Mathf.Clamp(currentHealth, 0, Health);
        HealthBar.value = currentHealth;
        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        Debug.Log("PlayerDEAD");
        GetComponent<Player_Movement>().enabled = false;

        // Disable Enemy AI on all enemies
        Enemy_AI[] enemies = Object.FindObjectsByType<Enemy_AI>(FindObjectsSortMode.None);
        foreach (Enemy_AI enemy in enemies)
        {
            enemy.enabled = false;
        }

        // Ensure SpawnEnemies method exists in GameManager
      

        Defeated_UI.SetActive(true); // Show defeated UI
    }
}
