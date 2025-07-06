using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeviceCellAssetAssigner : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI deviceNameText;
    public Image deviceImage;
    public Transform assetPreviewParent;

    [Header("Mappings")]
    public List<AssetMapping> assetMappings;

    [System.Serializable]
    public class AssetMapping
    {
        public string suffix;
        public Sprite image;
        public GameObject prefab3D;
    }

    public void SetupCell(string deviceName)
    {
        // Set the text label
        if (deviceNameText != null)
            deviceNameText.text = deviceName;

        // Match suffix to mapping
        foreach (var mapping in assetMappings)
        {
            if (deviceName.EndsWith(mapping.suffix))
            {
                // Set image if available
                if (deviceImage != null && mapping.image != null)
                    deviceImage.sprite = mapping.image;

                // Instantiate 3D asset if provided
                if (mapping.prefab3D != null && assetPreviewParent != null)
                {
                    // Destroy any old
                    foreach (Transform child in assetPreviewParent)
                        Destroy(child.gameObject);

                    Instantiate(mapping.prefab3D, assetPreviewParent);
                }

                return;
            }
        }

        // Default image / clear
        if (deviceImage != null)
            deviceImage.sprite = null;
    }
}
