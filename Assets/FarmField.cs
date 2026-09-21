using System.Collections.Generic;
using UnityEngine;

public class FarmField : MonoBehaviour
{
    public string fieldName = "初始农田";
    public bool isUnlocked = true;
    public int rows = 5;
    public int columns = 7;
    public float cellSpacing = 1f;
    public GameObject farmPlotPrefab;
    public int unlockCost = 200;
    public Transform plotParent;
    public Vector2 fieldSize = new Vector2(7f, 5f);

    private List<FarmPlot> plots = new List<FarmPlot>();

    public bool IsUnlocked => isUnlocked;

    private void Start()
    {
        if (isUnlocked)
        {
            GeneratePlots();
        }
    }

    private void GeneratePlots()
    {
        Transform parent = plotParent != null ? plotParent : transform;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                float x = (col - (columns - 1) / 2f) * cellSpacing;
                float y = (row - (rows - 1) / 2f) * cellSpacing;

                GameObject plotObject = Instantiate(farmPlotPrefab, parent);
                plotObject.transform.localPosition = new Vector3(x, y, 0f);

                FarmPlot plot = plotObject.GetComponent<FarmPlot>();
                plot.ownerField = this;
                plot.plotIndex = row * columns + col;
                plots.Add(plot);
            }
        }
    }

    public void Unlock()
    {
        isUnlocked = true;
        GeneratePlots();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(fieldSize.x, fieldSize.y, 0f));
    }
}
