using UnityEngine;

[RequireComponent(typeof(Transform))]
public class Spinner : MonoBehaviour
{
    [Header("Rotation Axis (Any Vector)")]
    [Tooltip("Choose any axis to rotate around (e.g. Y = (0,1,0), X+Z = (1,0,1))")]
    public Vector3 rotationAxis = Vector3.up;

    [Header("Rotation Speed (degrees per second)")]
    public float rotationSpeed = 30f;

    [Header("Space")]
    [Tooltip("Rotate in local or world space")]
    public Space rotationSpace = Space.Self;

    void Update()
    {
        if (rotationAxis == Vector3.zero)
            return;

        // Normalize the axis so rotation is consistent
        Vector3 normalizedAxis = rotationAxis.normalized;

        transform.Rotate(normalizedAxis, rotationSpeed * Time.deltaTime, rotationSpace);
    }
}
