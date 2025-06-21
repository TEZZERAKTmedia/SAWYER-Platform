using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDisplacer : MonoBehaviour
{
    [Header("Displacement Settings")]
    public float noiseScale = 1f;
    public float displacementStrength = 0.3f;
    public float animationSpeed = 1f;
    public bool animate = true;
    public float waveSpeed = 2f;

    private Mesh originalMesh;
    private Mesh displacedMesh;
    private Vector3[] originalVertices;
    private Vector3[] displacedVertices;

    void Start()
    {
        MeshFilter filter = GetComponent<MeshFilter>();

        originalMesh = filter.sharedMesh;
        displacedMesh = new Mesh
        {
            vertices = originalMesh.vertices,
            triangles = originalMesh.triangles,
            normals = originalMesh.normals,
            uv = originalMesh.uv,
            name = originalMesh.name + "_displaced"
        };

        filter.mesh = displacedMesh;

        originalVertices = (Vector3[])displacedMesh.vertices.Clone();
        displacedVertices = new Vector3[originalVertices.Length];

        DisplaceMesh();
    }

    void Update()
    {
        if (animate)
        {
            DisplaceMesh();
        }
    }

    void DisplaceMesh()
    {
        float t = Time.time * animationSpeed;
        Vector3 totalOffset = Vector3.zero;

        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 vertex = originalVertices[i];
            float distance = new Vector2(vertex.x, vertex.z).magnitude;

            float ripple = Mathf.Sin(distance * noiseScale * 0.5f - t * waveSpeed);
            float noise = Mathf.PerlinNoise(
                (vertex.x + t * 0.1f),
                (vertex.z + t * 0.1f)
            );
            float combined = ripple * 0.85f + (noise - 0.5f) * 0.15f;
            float wave = combined * displacementStrength;

            Vector3 displaced = vertex + Vector3.up * wave;

            displacedVertices[i] = displaced;
            totalOffset += displaced - vertex;
        }

        Vector3 averageOffset = totalOffset / originalVertices.Length;

        for (int i = 0; i < displacedVertices.Length; i++)
        {
            displacedVertices[i] -= averageOffset;
        }

        displacedMesh.vertices = displacedVertices;
        displacedMesh.RecalculateNormals();
        displacedMesh.RecalculateBounds();
    }


}
