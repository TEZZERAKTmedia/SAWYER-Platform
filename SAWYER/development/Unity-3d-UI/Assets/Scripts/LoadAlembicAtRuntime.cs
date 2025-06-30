using UnityEngine;

public class LoadAlembicAtRuntime : MonoBehaviour
{
    void Start()
    {
        GameObject uiPrefab = Resources.Load<GameObject>("3d_UI");
        if (uiPrefab != null)
        {
            Instantiate(uiPrefab);
            Debug.Log("Instantiated 3D_UI prefab from Resources.");
        }
        else
        {
            Debug.LogError("Failed to load 3D_UI prefab from Resources.");
        }
    }
}
