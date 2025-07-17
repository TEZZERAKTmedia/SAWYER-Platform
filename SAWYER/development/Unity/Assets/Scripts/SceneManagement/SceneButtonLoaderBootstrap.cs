using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]

public class SceneButtonLoaderBootstrap : MonoBehaviour
{
    [Header("Scene to load on button click")]
    [SerializeField] private string sceneName;

    private void Awake()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(LoadTargetScene);
    }

    private void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log($"[SceneButtonLoader] Loading scene: {sceneName}");
            SceneController.Instance?.LoadScene(sceneName);

        }
        else 
        {
            Debug.LogWarning("[SceneButtonLoader] No scene selected");
        }
    }

}