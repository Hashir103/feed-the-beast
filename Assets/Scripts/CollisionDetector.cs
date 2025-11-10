using UnityEngine;
using Unity.Behavior;

public class CollisionDetector : MonoBehaviour
{
    public Collider bowl1;
    public Collider bowl2;
    public BehaviorGraphAgent agent;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider == bowl1)
        {
            agent.SetVariableValue("Bowl1Full", true);
            Destroy(gameObject);
        }
        else if (collision.collider == bowl2)
        {
            agent.SetVariableValue("Bowl2Full", true);
            Destroy(gameObject);
        }
    }
}
