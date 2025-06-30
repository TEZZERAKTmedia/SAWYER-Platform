using UnityEngine;

public class ShaderTimeDriver : MonoBehaviour
{
    [Tooltip("Material using your displacement Shader Graph")]
    public Material targetMaterial;

    [Tooltip("Name of the property in the Shader Graph")]
    public string timePropertyName = "_TimeOffset";

    [Tooltip("Animation speed multiplier")]
    public float speed = 1.0f;

    private float timeValue = 0f;

    void Update()
    {
        if (targetMaterial == null || string.IsNullOrEmpty(timePropertyName)) return;

        timeValue += Time.deltaTime * speed;
        targetMaterial.SetFloat(timePropertyName, timeValue);
    }
}
