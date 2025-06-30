using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class VATAnimator : MonoBehaviour
{
    public Material mat;
    public float framesPerSecond = 30f;

    private float currentFrame = 0f;

#if UNITY_EDITOR
    private double lastEditorTime = 0;
#endif

    void Update()
    {
        if (mat == null) return;

        float deltaTime = 0f;

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            double currentTime = EditorApplication.timeSinceStartup;
            deltaTime = (float)(currentTime - lastEditorTime);
            lastEditorTime = currentTime;
        }
        else
#endif
        {
            deltaTime = Time.deltaTime;
        }

        // Pull values from the material
        float totalFrames = mat.GetFloat("_TotalFrames");

        currentFrame += deltaTime * framesPerSecond;
        if (currentFrame > totalFrames)
        {
            currentFrame = 0f;
        }

        mat.SetFloat("_Frame", currentFrame);
    }

    public float CurrentFrame => currentFrame;
}
