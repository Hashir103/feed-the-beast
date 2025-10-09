using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private float delayTime = 5f;
    private float moveSpeed = 3f;
    private bool canMove = false;
    public Transform bowl1;
    public Transform bowl2;
    public Transform player;
    private Transform target;
    public Transform startPosition;
    public Animator animator;


    void Start()
    {
        animator = transform.Find("Creep_mesh").GetComponent<Animator>();
        Invoke(nameof(LeaveIdle), delayTime);
    }

    void LeaveIdle()
    {
        animator.SetBool("isWalking", true);
        canMove = true;
        target = bowl1;
    }

    void LeaveEat1()
    {
        animator.SetBool("isEating", false);
        animator.SetBool("isWalking", true);
        canMove = true;
        target = bowl2;
    }

    void LeaveEat2()
    {
        animator.SetBool("isEating", false);
        animator.SetBool("isWalking", true);
        canMove = true;
        target = player;        
    }

    void LeaveAttack()
    {
        animator.SetBool("isAttacking", false);
        animator.SetBool("isWalking", true);
        canMove = true;
        target = startPosition;
    }

    void Update()
    {
        if (canMove && target != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                moveSpeed * Time.deltaTime
            );
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < 1f && target == bowl1)
            {
                canMove = false;
                animator.SetBool("isWalking", false);
                animator.SetBool("isEating", true);
                Invoke(nameof(LeaveEat1), delayTime);
            }
            else if (distance < 1f && target == bowl2)
            {
                canMove = false;
                animator.SetBool("isWalking", false);
                animator.SetBool("isEating", true);
                Invoke(nameof(LeaveEat2), delayTime);
            }
            else if (distance < 1f && target == player)
            {
                canMove = false;
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", true);
                Invoke(nameof(LeaveAttack), delayTime);
            }
            transform.LookAt(target);
        }
    }
}
