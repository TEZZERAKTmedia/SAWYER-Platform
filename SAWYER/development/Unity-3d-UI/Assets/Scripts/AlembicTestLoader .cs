using UnityEngine;
using UnityEngine.AddressableAssets;

public class AlembicTestLoader : MonoBehaviour
{
    void Start()
    {
        Addressables.InstantiateAsync("3d_UI").Completed += handle =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                Debug.Log("✅ Alembic prefab loaded!");
            }
            else
            {
                Debug.LogError("❌ Failed to load Alembic prefab.");
            }
        };
    }
}
