using UnityEngine;
using UnityEngine.AI;

public class Enemy_AI : MonoBehaviour
{
    [Header("General")]
    public Transform Player;
    public NavMeshAgent agent;
    public LayerMask Ground;
    public LayerMask PlayerMask;

    [Header("Health")]
    public float health = 100f;
    private float currentHealth;

    [Header("Patrolling")]
    public Vector3 navMeshAreaCenter;
    public Vector3 navMeshAreaSize;
    private Vector3 walkPoint;
    private bool walkPointSet;

    [Header("Chase & Detection")]
    public float sightRange = 30f;
    public float attackRange = 15f;
    private bool playerInSightRange;
    private bool playerInAttackRange;

    [Header("Attack")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootForce = 1000f;
    public float timeBetweenAttacks = 0.5f;
    private bool alreadyAttacked;
    public ParticleSystem muzzleFlash;
    public float range = 100f;
    public Transform orientation;
    public LayerMask playerlayer;

    void Start()
    {
        currentHealth = health;
        agent.speed = 15f;
        agent.acceleration = 60f;
        agent.angularSpeed = 1000f;
       
    }
    private void Awake()
    {
        if (firePoint == null)
        {
            firePoint = transform.Find("Fire Point");

        }
        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, PlayerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, PlayerMask);

        if (!playerInSightRange && !playerInAttackRange)
        {
            Patrolling();
        }
        else if (playerInSightRange)
        {
            Chase(); // Always chase when in sight

            if (playerInAttackRange)
            {
                ShootWhileChasing(); // Shoot if close enough
            }
        }
    }


    // ------------------ Patrolling ------------------
    void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    void SearchWalkPoint()
    {
        Vector3 randomDirection = new Vector3(
            Random.Range(-navMeshAreaSize.x / 2f, navMeshAreaSize.x / 2f),
            0,
            Random.Range(-navMeshAreaSize.z / 2f, navMeshAreaSize.z / 2f)
        );

        Vector3 targetPosition = navMeshAreaCenter + randomDirection;

        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }
    }

    // ------------------ Chase ------------------
    void Chase()
    {
        if (agent.isOnNavMesh)
            agent.SetDestination(Player.position);
    }


    // ------------------ Attack ------------------
    void ShootWhileChasing()
    {
        // Look at the player to aim
        Vector3 directionToPlayer = (Player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

        if (!alreadyAttacked)
        {
            Shoot();
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    void Shoot()
    {
        if (muzzleFlash != null) muzzleFlash.Play();

        Vector3 directionToPlayer = (Player.position - firePoint.position).normalized ;
        RaycastHit hit;

        float inaccuracy = 0.2f; // Increase this to miss more (0.1 = 10% randomness)
        Vector3 randomOffset = new Vector3(
            Random.Range(-inaccuracy, inaccuracy),
            Random.Range(-inaccuracy, inaccuracy),
            Random.Range(-inaccuracy, inaccuracy)
        );

        // Final direction with slight aim deviation
        Vector3 inaccurateDirection = (directionToPlayer + randomOffset).normalized;

        if (Physics.Raycast(firePoint.position, inaccurateDirection, out hit, range))
        {
            Debug.Log("Enemy Hit Object: " + hit.transform.name);

            // Check if hit any object tagged "Player"
            if (hit.collider.CompareTag("Player"))
            {
                // Search for Player_Movement on object or parent
                Player_Movement player = hit.collider.GetComponentInParent<Player_Movement>();

                if (player != null)
                {
                    Debug.Log("Player found! Applying damage.");
                    player.Player_Damage(10f);
                }
                else
                {
                    Debug.LogWarning("Hit player collider, but Player_Movement script not found!");
                }
            }
        }
    }

   


    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    // ------------------ Damage & Death ------------------
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("Enemy took damage: " + damage + " | Current Health: " + currentHealth);
        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        Player_Movement player = Player.GetComponent<Player_Movement>();
        if (player != null)
        {
            player.AddHealth(20f);
        }
        Destroy(gameObject);
        GameManager.Instance.AddScore(10);
    }

    // ------------------ Gizmos ------------------
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(navMeshAreaCenter, navMeshAreaSize);
    }
}
