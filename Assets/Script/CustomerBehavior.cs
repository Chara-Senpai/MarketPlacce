using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerBehavior : MonoBehaviour
{
    [Header("Path Points")]
    public Transform[] pathPoints; // 0 = spawn, 1 = middle, 2 = exit

    [Header("Shop References")]
    public Transform shelvesParent;
    public Transform counter;
    public Transform queueStart;

    [Header("Queue Settings")]
    public float spacing = 1.2f;

    [Header("Item Pickup")]
    public Transform itemHoldPoint;
    public float itemPickupOffsetY = 0.5f;
    private GameObject heldItem;

    [Header("Scanner")]
    public ScannerScript scanner; // Scanner calls OnItemScanned()

    [Header("Wait Times")]
    public float maxWaitTimeAtCounter = 5f;

    private NavMeshAgent agent;
    private Transform chosenShelfSide;  // FrontCenter or BackCenter
    private Transform chosenShelfRoot;  // Cube (actual shelf root)

    private enum State
    {
        WalkingPath, GoingToShelf, JoinQueue, MoveInQueue,
        GoingToCounter, WaitingAtCounter, GoingToExit, Leaving
    }
    private State currentState = State.WalkingPath;

    private int currentPointIndex = 0;
    private bool isWaiting = false;
    private bool itemScanned = false;

    private const float arrivalThreshold = 0.3f;
    public static List<CustomerBehavior> Queue = new List<CustomerBehavior>();

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GoToNextPathPoint();
    }

    void Update()
    {
        if (agent.pathPending) return;

        switch (currentState)
        {
            case State.WalkingPath:
            case State.GoingToShelf:
            case State.MoveInQueue:
            case State.GoingToCounter:
            case State.GoingToExit:
                if (agent.remainingDistance <= arrivalThreshold)
                    OnArrived();
                break;
        }

        if (currentState == State.JoinQueue)
            UpdateQueuePosition();
    }

    void LateUpdate()
    {
        // Keep item visually in hand
        if (heldItem != null && currentState != State.WaitingAtCounter)
        {
            heldItem.transform.position = itemHoldPoint.position + Vector3.up * itemPickupOffsetY;
            heldItem.transform.rotation = itemHoldPoint.rotation;
        }
    }

    // ------------------------------ ARRIVAL HANDLING ------------------------------

    void OnArrived()
    {
        switch (currentState)
        {
            case State.WalkingPath:
                HandleWalkingPath();
                break;

            case State.GoingToShelf:
                PickItemFromShelf();
                JoinQueueSystem();
                break;

            case State.GoingToCounter:
                StartCounterWait();
                break;

            case State.GoingToExit:
                Destroy(gameObject);
                break;
        }
    }

    // ------------------------------ WALKING PATH ------------------------------

    void HandleWalkingPath()
    {
        if (currentPointIndex == 1)
        {
            ChooseRandomShelf();
            currentState = State.GoingToShelf;
            return;
        }

        currentPointIndex++;
        if (currentPointIndex < pathPoints.Length)
            GoToNextPathPoint();
    }

    void GoToNextPathPoint()
    {
        agent.SetDestination(pathPoints[currentPointIndex].position);
    }

    // ------------------------------ CHOOSE SHELF ------------------------------

    void ChooseRandomShelf()
    {
        Transform shelfRoot = shelvesParent.GetChild(Random.Range(0, shelvesParent.childCount));
        chosenShelfRoot = shelfRoot;

        Transform front = shelfRoot.Find("FrontPoint");
        Transform back = shelfRoot.Find("BackPoint");

        chosenShelfSide = (Random.value < 0.5f) ? front : back;

        Debug.Log("NPC going to shelf side: " + chosenShelfSide.name);

        agent.SetDestination(chosenShelfSide.position);
    }

    // ------------------------------ PICK ITEM ------------------------------

    void PickItemFromShelf()
    {
        ShelfItem[] items = chosenShelfRoot.GetComponentsInChildren<ShelfItem>(true);

        if (items.Length == 0)
        {
            Debug.LogWarning("NO items found on shelf: " + chosenShelfRoot.name);
            return;
        }

        ShelfItem chosen = items[Random.Range(0, items.Length)];
        heldItem = chosen.gameObject;

        // Pick up item
        heldItem.transform.SetParent(itemHoldPoint);
        heldItem.transform.localPosition = new Vector3(0, itemPickupOffsetY, 0);
        heldItem.transform.localRotation = Quaternion.identity;

        Rigidbody rb = heldItem.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Debug.Log("NPC picked item: " + heldItem.name);
    }

    // ------------------------------ QUEUE SYSTEM ------------------------------

    void JoinQueueSystem()
    {
        currentState = State.JoinQueue;
        Queue.Add(this);
        UpdateQueuePosition();
    }

    void UpdateQueuePosition()
    {
        int index = Queue.IndexOf(this);
        Vector3 newPos = queueStart.position - queueStart.forward * (index * spacing);
        agent.SetDestination(newPos);

        if (index == 0 && agent.remainingDistance < arrivalThreshold)
        {
            Queue.RemoveAt(0);
            GoToCounter();
        }
        else
        {
            currentState = State.MoveInQueue;
        }
    }

    void GoToCounter()
    {
        currentState = State.GoingToCounter;
        agent.SetDestination(counter.position);
    }

    // ------------------------------ COUNTER & SCANNING ------------------------------

    void StartCounterWait()
    {
        if (isWaiting) return;

        isWaiting = true;
        currentState = State.WaitingAtCounter;

        // Place item on counter
        if (heldItem != null)
        {
            heldItem.transform.SetParent(counter);
            heldItem.transform.position = counter.position + Vector3.up * 0.5f;
            heldItem.transform.rotation = Quaternion.identity;

            Rigidbody rb = heldItem.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;
        }

        StartCoroutine(WaitAtCounter());
    }

    IEnumerator WaitAtCounter()
    {
        float timer = 0f;

        while (timer < maxWaitTimeAtCounter)
        {
            if (itemScanned) break; // Scanner will call OnItemScanned()
            timer += Time.deltaTime;
            yield return null;
        }

        LeaveCounter();
    }

    public void OnItemScanned()
    {
        itemScanned = true;
        LeaveCounter();
    }

    void LeaveCounter()
    {
        isWaiting = false;
        currentState = State.GoingToExit;
        agent.SetDestination(pathPoints[2].position);
        heldItem = null;
    }
}
