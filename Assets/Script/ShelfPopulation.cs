using UnityEngine;

public class ShelfPopulation : MonoBehaviour
{
    public GameObject itemPrefab;

    public Transform frontCenter;  // Center of front side
    public Transform backCenter;   // Center of back side

    public int rows = 3;
    public int columns = 3;
    public float spacingX = 0.3f;
    public float spacingY = 0.3f;

    void Start()
    {
        PopulateSide(frontCenter, false);
        PopulateSide(backCenter, true);
    }

    void PopulateSide(Transform centerPoint, bool flipRotation)
    {
        if (centerPoint == null) return;

        // Calculate offset to top-left corner
        float offsetX = (columns - 1) * spacingX / 2f;
        float offsetY = (rows - 1) * spacingY / 2f;

        Vector3 topLeft = centerPoint.position - centerPoint.right * offsetX + centerPoint.up * offsetY;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Vector3 spawnPos =
                    topLeft +
                    (centerPoint.right * (c * spacingX)) -
                    (centerPoint.up * (r * spacingY));

                Quaternion rot = flipRotation
                    ? Quaternion.Euler(centerPoint.eulerAngles + new Vector3(0, 180, 0))
                    : centerPoint.rotation;

                GameObject newItem = Instantiate(itemPrefab, spawnPos, rot);
                newItem.transform.SetParent(transform);
            }
        }
    }
}

