using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevicesGridController : MonoBehaviour
{
    [Header("Device Data")]
    public List<string> deviceNames = new List<string>();
    public List<Sprite> deviceTypeImages; // Matching suffix -> Sprite

    [Header("Grid Slots")]
    public List<CellRenderer> gridCells;

    [Header("Add Device Event")]
    public Button.ButtonClickedEvent onAddDeviceClicked;

    [Header("Max Cells")]
    public int maxCells = 6;

    void Start()
    {
        RenderGrid();
    }

    public void RenderGrid()
    {
        for (int i = 0; i < maxCells; i++)
        {
            if (i < deviceNames.Count)
            {
                // Render device
                string deviceName = deviceNames[i];
                Sprite icon = GetIconForDevice(deviceName);
                gridCells[i].ShowDevice(deviceName, icon);
            }
            else if (i == deviceNames.Count)
            {
                // Render Add button
                gridCells[i].ShowAddButton(() =>
                {
                    onAddDeviceClicked.Invoke();
                });
            }
            else
            {
                // Empty cell
                gridCells[i].ShowEmpty();
            }
        }
    }

    Sprite GetIconForDevice(string deviceName)
    {
        // Example: match suffix to image
        foreach (var sprite in deviceTypeImages)
        {
            if (deviceName.EndsWith(sprite.name))
                return sprite;
        }
        return null;
    }
}
