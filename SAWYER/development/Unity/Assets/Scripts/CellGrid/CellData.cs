using UnityEngine;

[System.Serializable]
public class CellData
{
    public GameObject assignedObject;
    public RectTransform cellRect;
    public int row;
    public int col;
    public string cellId;

    public CellData(int r, int c)
    {
        row = r;
        col = c;
        cellId = $"Cell_{r}_{c}";
    }
}
