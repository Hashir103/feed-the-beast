using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float angularSpeed = 720f;
    [SerializeField] private float stoppingDistance = 0.35f;

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
}
