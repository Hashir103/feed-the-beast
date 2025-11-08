using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Behavior Timing")]
    [SerializeField] private float delayTime = 5f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 0.2f;

    [Header("Targets")]
    public Transform bowl1;
    public Transform bowl2;
    public Transform player;
    public Transform startPosition;

    private Transform target;
    private bool canMove = false;

    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        animator = transform.Find("Creep_mesh").GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            // Automatic NavMeshAgent movement handling
            agent.speed = moveSpeed;
            agent.angularSpeed = 720f;        // controls turn speed
            agent.acceleration = 16f;         // faster response
            agent.autoBraking = false;        // smooth corners
            agent.radius = 0.25f;             // reduce to avoid clipping walls
            agent.stoppingDistance = stoppingDistance;
            agent.updatePosition = true;
            agent.updateRotation = true;      // let Unity handle rotation
            agent.autoRepath = true;
        }

        Invoke(nameof(LeaveIdle), delayTime);
    }

    void LeaveIdle()
    {
        animator.SetBool("isWalking", true);
        canMove = true;
        target = bowl1;
        SetDestination();
    }

    void LeaveEat1()
    {
        animator.SetBool("isEating", false);
        animator.SetBool("isWalking", true);
        canMove = true;
        target = bowl2;
        SetDestination();
    }

    void LeaveEat2()
    {
        animator.SetBool("isEating", false);
        animator.SetBool("isWalking", true);
        canMove = true;
        target = player;
        SetDestination();
    }

    void LeaveAttack()
    {
        animator.SetBool("isAttacking", false);
        animator.SetBool("isWalking", true);
        canMove = true;
        target = startPosition;
        SetDestination();
    }

    void Update()
    {
        if (!canMove || target == null || agent == null)
            return;

        // If agent somehow stopped, resume it
        if (agent.isStopped)
            agent.isStopped = false;

        // Update the destination if needed
        if (agent.destination != target.position)
            SetDestination();

        // Check if arrived
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
        {
            HandleArrival();
        }

        // Update animation based on movement
        bool isMoving = agent.velocity.sqrMagnitude > 0.01f;
        animator.SetBool("isWalking", isMoving);
    }

    private void SetDestination()
    {
        if (agent == null || target == null) return;

        // Make sure target is on NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(target.position, out hit, 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            agent.SetDestination(target.position);
        }
    }

    private void HandleArrival()
    {
        if (target == bowl1)
        {
            canMove = false;
            animator.SetBool("isWalking", false);
            animator.SetBool("isEating", true);
            Invoke(nameof(LeaveEat1), delayTime);
        }
        else if (target == bowl2)
        {
            canMove = false;
            animator.SetBool("isWalking", false);
            animator.SetBool("isEating", true);
            Invoke(nameof(LeaveEat2), delayTime);
        }
        else if (target == player)
        {
            canMove = false;
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);
            Invoke(nameof(LeaveAttack), delayTime);
        }
        else if (target == startPosition)
        {
            canMove = false;
            animator.SetBool("isWalking", false);
            animator.SetBool("isEating", false);
            animator.SetBool("isAttacking", false);
            // Optionally loop again if desired
            Invoke(nameof(LeaveIdle), delayTime);
        }
    }
}
