using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class MeshMorpher : MonoBehaviour
{
    [Header("Morph Targets")]
    public List<Mesh> targetMeshes = new List<Mesh>();
    public float morphDuration = 3f;
    public bool loop = true;

    private MeshFilter meshFilter;
    private Mesh baseMesh;
    private Vector3[] baseVertices;
    private Vector3[] workingVertices;
    private Vector3[] targetVertices;

    private int currentTargetIndex = 0;
    private float morphTimer = 0f;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        baseMesh = meshFilter.mesh;

        baseVertices = baseMesh.vertices;
        workingVertices = new Vector3[baseVertices.Length];
        meshFilter.mesh = new Mesh
        {
            vertices = baseVertices,
            triangles = baseMesh.triangles,
            uv = baseMesh.uv,
            normals = baseMesh.normals,
            name = "MorphedMesh"
        };

        if (targetMeshes.Count > 0)
            SetTargetMesh(0);
    }

    void Update()
    {
        if (targetMeshes.Count == 0 || targetVertices == null)
            return;

        morphTimer += Time.deltaTime;
        float t = morphTimer / morphDuration;

        for (int i = 0; i < baseVertices.Length; i++)
        {
            workingVertices[i] = Vector3.Lerp(baseVertices[i], targetVertices[i], t);
        }

        meshFilter.mesh.vertices = workingVertices;
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.RecalculateBounds();

        if (t >= 1f)
        {
            morphTimer = 0f;
            currentTargetIndex++;

            if (currentTargetIndex >= targetMeshes.Count)
            {
                if (loop)
                    currentTargetIndex = 0;
                else
                    enabled = false;
            }

            SetTargetMesh(currentTargetIndex);
        }
    }

    void SetTargetMesh(int index)
    {
        baseVertices = meshFilter.mesh.vertices;
        targetVertices = targetMeshes[index].vertices;

        if (targetVertices.Length != baseVertices.Length)
        {
            Debug.LogError("Target mesh does not match vertex count with base mesh.");
            enabled = false;
        }
    }
}
