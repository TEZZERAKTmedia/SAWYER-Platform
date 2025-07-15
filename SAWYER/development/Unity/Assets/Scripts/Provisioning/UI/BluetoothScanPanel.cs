using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BluetoothScanPanel : MonoBehaviour
{
    [SerializeField] private ProvisioningUIController controller;
    [SerializeField] private Transform deviceListContainer;
    [SerializeField] private GameObject deviceButtonPrefab;

    public void Populate(List<string> deviceNames)
    {
        foreach (Transform child in deviceListContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var name in deviceNames)
        {
            var btnObj = Instantiate(deviceButtonPrefab, deviceListContainer);
            btnObj.GetComponentInChildren<Text>().text = name;
            btnObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnDeviceSelected(name);
            });
        }
    }

    private void OnDeviceSelected(string deviceName)
    {
        Debug.Log($"[BluetoothScanPanel] User chose device: {deviceName}");
        controller.OnDeviceSelected();
    }
}
