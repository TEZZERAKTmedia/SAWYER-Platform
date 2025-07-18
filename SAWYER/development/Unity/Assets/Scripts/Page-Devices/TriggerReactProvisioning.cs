using UnityEngine;
using UnityEngine.UI;

#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

[RequireComponent(typeof(Button))]
public class RNBridge : MonoBehaviour
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void openProvisioningFlow();
#endif

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(TriggerProvisioning);
    }

    private void TriggerProvisioning()
    {
#if UNITY_IOS && !UNITY_EDITOR
        Debug.Log("[Unity] Calling openProvisioningFlow() on iOS");
        openProvisioningFlow();
#else
        Debug.Log("[Unity] Would call openProvisioningFlow() on iOS");
#endif
    }
}
