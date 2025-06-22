using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class CameraMoveTarget
{
    public GameObject clickableObject;
    public Transform destination;
}

public class CameraMenuController : MonoBehaviour
{
    [Header("Camera Movement Settings")]
    public float moveSpeed = 3f;
    public float stopDistance = 0.01f;

    [Header("Optional: Track Target While Moving")]
    public Transform trackTarget;
    public float trackSpeed = 5f;

    [Header("Clickable Pairs")]
    public List<CameraMoveTarget> targets = new List<CameraMoveTarget>();

    private Transform moveTarget;
    private bool isMoving = false;

    void Start()
    {
        foreach (var pair in targets)
        {
            AddClickListener(pair.clickableObject, pair.destination);
        }
    }

    void Update()
    {
        if (isMoving && moveTarget != null)
        {
            transform.position = Vector3.Lerp(transform.position, moveTarget.position, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, moveTarget.rotation, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, moveTarget.position) < stopDistance)
                isMoving = false;
        }

        if (trackTarget != null)
        {
            Quaternion look = Quaternion.LookRotation(trackTarget.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, trackSpeed * Time.deltaTime);
        }
    }

    public void MoveCameraTo(Transform destination)
    {
        moveTarget = destination;
        isMoving = true;
    }

    private void AddClickListener(GameObject clickable, Transform target)
    {
        if (clickable.GetComponent<Collider>() == null)
        {
            Debug.LogWarning($"Clickable object '{clickable.name}' needs a Collider component to detect clicks.");
            return;
        }

        EventTrigger trigger = clickable.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = clickable.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };

        entry.callback.AddListener((eventData) => MoveCameraTo(target));
        trigger.triggers.Add(entry);
    }
}
