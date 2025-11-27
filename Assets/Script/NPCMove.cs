using UnityEngine;
using UnityEngine.AI;

public class NPCMove : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found!");
            enabled = false;
            return;
        }

        agent.updateRotation = false; // manual rotation
        agent.stoppingDistance = 1.5f; // prevents overlap sliding
    }

    void Update()
    {
        if (target == null) return;

        // Force the agent to move directly toward player in straight line
        agent.SetDestination(target.position);

        // SNAP ROTATION instantly
        Vector3 toPlayer = (target.position - transform.position);
        toPlayer.y = 0; // keep upright
        transform.rotation = Quaternion.LookRotation(toPlayer.normalized);
    }
}
