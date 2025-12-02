using UnityEngine;
using UnityEngine.UI;

public class Pickup : MonoBehaviour
{
    public Transform hand;             // Where item is held
    public float interactRange = 3f;

    public GameObject pickupPrompt;    // UI text for "Press E to pick up"

    private ItemPickUp itemInHand;
    private ItemPickUp itemLookedAt;

    void Update()
    {
        CheckForItemLook();

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickup();
        }
    }

    void CheckForItemLook()
    {
        // Ray forward from camera
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            ItemPickUp item = hit.collider.GetComponent<ItemPickUp>();

            if (item != null)
            {
                itemLookedAt = item;
                pickupPrompt.SetActive(true);
                return;
            }
        }

        // If no item looked at
        itemLookedAt = null;
        pickupPrompt.SetActive(false);
    }

    void TryPickup()
    {
        if (itemLookedAt == null)
            return;

        // If holding something, return it
        if (itemInHand != null)
        {
            ReturnToOriginalPosition(itemInHand);
        }

        // Pick up the looked-at item
        itemInHand = itemLookedAt;
        PutInHand(itemInHand);

        pickupPrompt.SetActive(false);
    }

    void PutInHand(ItemPickUp item)
    {
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        item.transform.SetParent(hand);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
    }

    void ReturnToOriginalPosition(ItemPickUp item)
    {
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = false;

        item.transform.SetParent(null);
        item.transform.position = item.originalPosition;
        item.transform.rotation = item.originalRotation;
    }
}
