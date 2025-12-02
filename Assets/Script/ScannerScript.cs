using System.Collections;
using UnityEngine;
using TMPro;

public class ScannerScript : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI scanPromptPrefab;  // Prefab for floating text
    public Transform canvas;                   // Parent canvas
    public float promptDuration = 2f;
    public Vector3 promptOffset = new Vector3(0, 2f, 0);

    public void ScanItem(GameObject item)
    {
        if (item == null) return;

        Debug.Log("Scanner: Item scanned -> " + item.name);

        // Show floating UI text
        if (scanPromptPrefab != null)
        {
            TextMeshProUGUI promptInstance = Instantiate(scanPromptPrefab, canvas);
            promptInstance.text = "Scanned: " + item.name;

            // Convert world position to screen position
            Vector3 screenPos = Camera.main.WorldToScreenPoint(item.transform.position + promptOffset);
            promptInstance.transform.position = screenPos;

            // Fade and destroy
            CustomerBehavior npc = item.GetComponent<CustomerBehavior>();
            if (npc != null)
                npc.StartCoroutine(FadePrompt(promptInstance));
        }

        // Notify NPC
        CustomerBehavior customer = item.GetComponent<CustomerBehavior>();
        if (customer != null)
        {
            customer.OnItemScanned();
        }
    }

    private IEnumerator FadePrompt(TextMeshProUGUI prompt)
    {
        float timer = 0f;
        while (timer < promptDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(prompt.gameObject);
    }
}
