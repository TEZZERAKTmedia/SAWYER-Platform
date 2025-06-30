using UnityEngine;

public class InnerSpawner : MonoBehaviour
{
    [Tooltip("Assign the INNER prefab here")]
    public GameObject innerPrefab;

    void Start()
    {
        if (innerPrefab != null)
        {
            Instantiate(innerPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("INNER prefab not assigned in the Inspector.");
        }
    }
}
