// Place this in Assets/Editor/BarycentricMesh.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BarycentricMesh : MonoBehaviour
{
    [ContextMenu("Add Barycentric UVs")]
    void AddBarycentrics()
    {
        var mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null)
        {
            Debug.LogWarning("No MeshFilter found!");
            return;
        }

        Mesh mesh = Instantiate(mf.sharedMesh);
        int[] tris = mesh.triangles;
        Vector3[] verts = mesh.vertices;

        // Prepare a list of 3-component UVs for each vertex
        List<Vector3> bary = new List<Vector3>(new Vector3[verts.Length]);

        // For each triangle, assign each corner a unique barycentric coordinate
        for (int i = 0; i < tris.Length; i += 3)
        {
            int i0 = tris[i + 0];
            int i1 = tris[i + 1];
            int i2 = tris[i + 2];

            bary[i0] = new Vector3(1, 0, 0);
            bary[i1] = new Vector3(0, 1, 0);
            bary[i2] = new Vector3(0, 0, 1);
        }

        // Write into UV2 (channel 1)
        mesh.SetUVs(1, bary);

        mf.sharedMesh = mesh;
        Debug.Log("Barycentric UVs added to mesh: " + mesh.name);
    }
}
