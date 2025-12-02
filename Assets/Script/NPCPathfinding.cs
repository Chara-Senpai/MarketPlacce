using UnityEngine;
using UnityEngine.AI;

public class NPCPathfinding : MonoBehaviour
{
    public Transform[] pathPoints;
    private NavMeshAgent agent;
    private int currentPointIndex = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GoToNextPoint();
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            currentPointIndex++;
            if (currentPointIndex >= pathPoints.Length)
            {
                Destroy(gameObject);
            }
            else
            {
                GoToNextPoint();
            }
        }
    }

    void GoToNextPoint()
    {
        Vector3 target = pathPoints[currentPointIndex].position;

        // Sample the nearest valid NavMesh point to avoid walls or invalid areas
        NavMeshHit hit;
        if (NavMesh.SamplePosition(target, out hit, 1.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            Debug.LogWarning("Invalid path point! Cannot find NavMesh position near: " + target);
        }
    }
}
