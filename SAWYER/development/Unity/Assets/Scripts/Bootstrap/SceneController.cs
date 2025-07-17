using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        Debug.Log($"[SceneController] Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void LoadSceneFromMessages(string sceneName)
    {
        LoadScene(sceneName);
    }
}