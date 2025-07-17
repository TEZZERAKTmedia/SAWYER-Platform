using UnityEngine;
using UnityEngine.SceneManagement;


public class BootstrapLoader : MonoBehaviour
{
    [Header("Scene to load on start")]
    [SerializeField] private string targetSceneName;

    void Start()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            Debug.Log($"[BootstrapLoader] Loading scene: {targetSceneName}");
            SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
        }
        else{
            Debug.Log("[BootstrapLoader] No target scene specified, nothing to load.");
        }
    }
}