using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class PlaneClassifier : MonoBehaviour
{
    [Header("Dependencies")]
    public ARPlaneManager planeManager;
    public Transform roomRoot;

    [Header("Material Feedback")]
    public Material floorMaterial;
    public Material wallMaterial;
    public Material ceilingMaterial;
    public Material objectMaterial;
    public Material otherMaterial;

    [Header("Classification Thresholds (in meters)")]
    public float floorThreshold = 0.05f;     // ~2 inches
    public float ceilingMinY = 2.4f;         // ~8 feet
    public float wallMinHeight = 2.4f;       // ~8 feet

    private void OnEnable()
    {
#pragma warning disable CS0618
        planeManager.planesChanged += OnPlanesChanged;
#pragma warning restore CS0618
    }

    private void OnDisable()
    {
#pragma warning disable CS0618
        planeManager.planesChanged -= OnPlanesChanged;
#pragma warning restore CS0618
    }

#pragma warning disable CS0618
    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        foreach (var plane in args.added)
        {
            ClassifyAndOrganizePlane(plane);
        }
    }
#pragma warning restore CS0618

    private void ClassifyAndOrganizePlane(ARPlane plane)
    {
        Vector3 normal = plane.transform.up;
        float yPos = plane.transform.position.y;
        float height = plane.size.y;

        string category;
        Material matToApply;

        if (IsHorizontal(normal))
        {
            if (yPos < floorThreshold)
            {
                category = "Floor";
                matToApply = floorMaterial;
            }
            else if (yPos > ceilingMinY)
            {
                category = "Ceiling";
                matToApply = ceilingMaterial;
            }
            else
            {
                category = "Other_Horizontal";
                matToApply = otherMaterial;
            }
        }
        else if (IsVertical(normal))
        {
            if (height > wallMinHeight)
            {
                category = "Wall";
                matToApply = wallMaterial;
            }
            else
            {
                category = "Object";
                matToApply = objectMaterial;
            }
        }
        else
        {
            category = "Uncategorized";
            matToApply = otherMaterial;
        }

        var parentGroup = GetOrCreateGroup(category);
        plane.transform.SetParent(parentGroup);
        plane.gameObject.name = $"{category}_Plane_{plane.trackableId}";

        var meshRenderer = plane.GetComponent<MeshRenderer>();
        if (meshRenderer != null && matToApply != null)
        {
            meshRenderer.material = matToApply;
        }
    }

    private bool IsHorizontal(Vector3 normal)
    {
        return Vector3.Dot(normal, Vector3.up) > 0.9f;
    }

    private bool IsVertical(Vector3 normal)
    {
        return Mathf.Abs(Vector3.Dot(normal, Vector3.up)) < 0.1f;
    }

    private Transform GetOrCreateGroup(string groupName)
    {
        Transform group = roomRoot.Find(groupName);
        if (group == null)
        {
            GameObject groupGO = new GameObject(groupName);
            groupGO.transform.SetParent(roomRoot);
            group = groupGO.transform;
        }
        return group;
    }
}
