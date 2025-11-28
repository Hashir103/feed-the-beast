using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float angularSpeed = 720f;
    [SerializeField] private float stoppingDistance = 0.35f;

    [Header("Attack Settings")]
    [SerializeField] private PlayerHealth playerHealth; // drag player here
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackRange = 1.5f;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform lookTarget;
    private float cachedSpeed;

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
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            if (Mathf.Abs(agent.speed - cachedSpeed) > 0.01f)
                agent.speed = cachedSpeed;

            if (animator != null)
            {
                float normalizedSpeed = Mathf.Clamp01(agent.velocity.magnitude / cachedSpeed);
                animator.SetFloat("SpeedMagnitude", normalizedSpeed);
                animator.SetBool("isWalking", normalizedSpeed > 0.05f);
            }
        }
    }
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
