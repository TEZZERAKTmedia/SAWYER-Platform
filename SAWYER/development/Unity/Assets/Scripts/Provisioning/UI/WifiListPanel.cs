using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WifiListPanel : MonoBehaviour
{
    [SerializeField] private ProvisioningUIController controller;
    [SerializeField] private Transform networkListContainer;
    [SerializeField] private GameObject networkButtonPrefab;

    public void Populate(List<string> networkNames)
    {
        foreach (Transform child in networkListContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var name in networkNames)
        {
            var btnObj = Instantiate(networkButtonPrefab, networkListContainer);
            btnObj.GetComponentInChildren<Text>().text = name;
            btnObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnNetworkSelected(name);
            });
        }
    }

    private void OnNetworkSelected(string ssid)
    {
        Debug.Log($"[WifiListPanel] User chose network: {ssid}");
        controller.OnWifiNetworkSelected();
    }
}
