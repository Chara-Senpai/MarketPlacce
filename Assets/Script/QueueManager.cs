using UnityEngine;
using System.Collections.Generic;

public class QueueManager : MonoBehaviour
{
    public Transform[] queueSlots;
    private Queue<CustomerBehavior> waitingQueue = new Queue<CustomerBehavior>();

    public Vector3 GetNextAvailableSlot(CustomerBehavior customer)
    {
        waitingQueue.Enqueue(customer);
        int index = waitingQueue.Count - 1; // first in line = index 0
        if (index >= queueSlots.Length) index = queueSlots.Length - 1; // clamp
        return queueSlots[index].position;
    }

    public void RemoveFromQueue(CustomerBehavior customer)
    {
        waitingQueue.Dequeue();
    }
}

