using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float angularSpeed = 720f;
    [SerializeField] private float stoppingDistance = 0.35f;

    [Header("Attack Settings")]
    [SerializeField] private PlayerHealth playerHealth; // drag Player here
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1f; // seconds between hits

    private NavMeshAgent agent;
    private Animator animator;
    private float cachedSpeed;
    private float lastAttackTime = -999f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = transform.Find("Creep_mesh").GetComponent<Animator>();

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.angularSpeed = angularSpeed;
            agent.acceleration = 30f;
            agent.stoppingDistance = stoppingDistance;
            agent.autoBraking = true;
            agent.radius = 0.25f;
            agent.updateRotation = true;
            agent.updatePosition = true;
            agent.autoRepath = true;
        }

        if (animator != null)
            animator.applyRootMotion = false;

        cachedSpeed = moveSpeed;
    }

    void Update()
    {
        // Movement + animation speed sync
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            if (Mathf.Abs(agent.speed - cachedSpeed) > 0.01f)
                agent.speed = cachedSpeed;

            if (animator != null)
            {
                float normalizedSpeed =
                    Mathf.Clamp01(agent.velocity.magnitude / cachedSpeed);

                animator.SetFloat("SpeedMagnitude", normalizedSpeed);
                animator.SetBool("isWalking", normalizedSpeed > 0.05f);
            }
        }

        HandleAutoAttack();
    }

    // Automatically attack when the animator says we are attacking
    private void HandleAutoAttack()
    {
        if (animator == null || playerHealth == null) return;

        bool isAttacking = animator.GetBool("isAttacking");

        if (isAttacking)
        {
            Debug.Log($"EnemyAI {name} is damaging {playerHealth.gameObject.name}");
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                float dist = Vector3.Distance(
                    transform.position,
                    playerHealth.transform.position
                );

                if (dist <= attackRange)
                {
                    playerHealth.TakeDamage(attackDamage);
                    lastAttackTime = Time.time;
                }
            }
        }
    }

    // Optional: Called if you ever want to hit manually (animation events, etc.)
    public void AttackHit()
    {
        if (playerHealth == null) return;

        float dist = Vector3.Distance(transform.position, playerHealth.transform.position);
        if (dist <= attackRange)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }
}
