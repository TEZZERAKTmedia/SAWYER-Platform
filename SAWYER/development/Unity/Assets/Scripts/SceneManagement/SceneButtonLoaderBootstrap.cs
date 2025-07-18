using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]

public class SceneButtonLoaderBootstrap : MonoBehaviour
{
    [Header("Scene to load on button click")]
    [SerializeField] private string sceneName;

    [Header("Preserve current scene state (load additive)")]
    [SerializeField] private bool preserveSceneState = false;

    private void Awake()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(LoadTargetScene);
    }

    private void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log($"[SceneButtonLoader] Loading scene: {sceneName}, Preserve current scene: {preserveSceneState}");
            if (preserveSceneState)
            {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            }
            else
            {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            }

        }
        else 
        {
            Debug.LogWarning("[SceneButtonLoader] No scene selected");
        }
    }

}