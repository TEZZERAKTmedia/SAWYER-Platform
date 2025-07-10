using UnityEngine;

[ExecuteAlways]
public class VATMetaLoader : MonoBehaviour
{
    [Tooltip("Drag your INNERnew_VAT_metadata.json here")]
    public TextAsset metadataFile;

    [System.Serializable]
    public class VATMeta
    {
        public int     vertex_count;
        public int     frame_count;
        public float[] position_min_bounds;
        public float[] position_max_bounds;
    }

    void OnValidate()
    {
        if (metadataFile == null) return;
        ApplyMeta();
    }

    void OnEnable()
    {
        // Also apply at runtime start
        if (Application.isPlaying)
            ApplyMeta();
    }

    private void ApplyMeta()
    {
        // Parse JSON
        var meta = JsonUtility.FromJson<VATMeta>(metadataFile.text);
        if (meta == null ||
            meta.position_min_bounds == null ||
            meta.position_max_bounds == null)
            return;

        // Grab renderer & choose the right material reference
        var rend = GetComponent<Renderer>();
        if (rend == null) return;

        // In edit mode/write-through, use sharedMaterial to avoid leaks.
        // In play mode, you can use material if you want an instance.
        var mat = Application.isPlaying
            ? rend.material
            : rend.sharedMaterial;
        if (mat == null) return;

        // Set the shader properties (must match your Blackboard References)
        mat.SetFloat("_VertexCount",  meta.vertex_count);
        mat.SetFloat("_TotalFrames",  meta.frame_count);

        var min = meta.position_min_bounds;
        var max = meta.position_max_bounds;
        mat.SetVector("_MinBounds", new Vector4(min[0], min[1], min[2], 0f));
        mat.SetVector("_MaxBounds", new Vector4(max[0], max[1], max[2], 0f));
    }
}
