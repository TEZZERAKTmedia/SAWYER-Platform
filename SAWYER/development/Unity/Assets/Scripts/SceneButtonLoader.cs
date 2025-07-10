using UnityEngine;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneButtonLoader : MonoBehaviour
{
    [SerializeField] private string sceneName;

    #if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;
    private void OnValidate()
    {
        if(sceneAsset !=null) {
            sceneName = sceneAsset.name;
        }
    }

    #endif

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is not set!");
        }
    }


}