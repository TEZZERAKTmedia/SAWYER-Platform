using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class EnvironmentsGridPopulation : MonoBehaviour
{
    [Header("Grid References")]
    public Transform gridContainer;
    public GameObject environmentCellPrefab;
    public GameObject addButtonPrefab;

    [Header("Settings")]
    public int maxCells = 6;

    private List<string> environmentNames = new List<string>();

    void Start()
    {
        LoadEnvironments();
        PopulateGrid();
    }

    void LoadEnvironments()
    {
        environmentNames.Clear();

        // Example root path
        string rootPath = Application.persistentDataPath + "/Environments";

        // Just add dummy data for now:
        environmentNames.Add("Test Environment 1");
        environmentNames.Add("Test Environment 2");

        Debug.Log("[EnvironmentsGridPopulation] Environments Loaded.");
    }

    void PopulateGrid()
    {
        // Clear existing
        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }

        // Add environment cells
        foreach (string env in environmentNames)
        {
            GameObject cell = Instantiate(environmentCellPrefab, gridContainer);
            // TODO: Set name/thumbnail/etc.
        }

        // Add "Add" button at end
        Instantiate(addButtonPrefab, gridContainer);
    }
}
