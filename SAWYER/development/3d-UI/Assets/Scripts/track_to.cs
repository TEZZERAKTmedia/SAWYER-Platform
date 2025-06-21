using UnityEngine;

public class CameraLookAt : MonoBehaviour

{
    public Transform target; 

    void Update()
    {
        if(target != null)
        {
            // Make the camera look at the target
            transform.LookAt(target.position);
        }
    }
}
