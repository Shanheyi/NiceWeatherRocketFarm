using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public SeedSelector seedSelector;
    public gosteavego playerController;
    public LayerMask farmPlotLayer;
    public KeyCode plantKey = KeyCode.E;
    public KeyCode harvestKey = KeyCode.F;
    public RectTransform plotHighlight;
    public Camera mainCamera;
    public CropData eggplantData;

    private List<FarmPlot> allPlots = new List<FarmPlot>();
    private float refreshTimer = 0f;

    private void Start()
    {
        RefreshPlotList();
    }

    private void RefreshPlotList()
    {
        if (allPlots == null)
        {
            allPlots = new List<FarmPlot>();
        }

        allPlots.Clear();
        allPlots.AddRange(FindObjectsOfType<FarmPlot>());
    }

    private void Update()
    {
        refreshTimer += Time.deltaTime;
        if (refreshTimer > 1f)
        {
            refreshTimer = 0f;
            RefreshPlotList();
        }

        if (Input.GetKeyDown(plantKey))
        {
            PlantInRange();
        }

        if (Input.GetKeyDown(harvestKey))
        {
            HarvestInRange();
        }

        UpdateHighlight();
    }

    private float QuantizeToCellCenter(float position)
    {
        return Mathf.Round(position - 0.5f) + 0.5f;
    }

    private List<FarmPlot> GetPlotsInRange()
    {
        List<FarmPlot> result = new List<FarmPlot>();
        if (playerController == null || allPlots == null)
        {
            return result;
        }

        int rangeSize = playerController.GetFarmRangeSize();
        int half = rangeSize / 2;

        float playerCellX = QuantizeToCellCenter(transform.position.x);
        float playerCellY = QuantizeToCellCenter(transform.position.y);

        foreach (FarmPlot plot in allPlots)
        {
            if (plot == null)
            {
                continue;
            }

            float plotCellX = QuantizeToCellCenter(plot.transform.position.x);
            float plotCellY = QuantizeToCellCenter(plot.transform.position.y);

            float dx = (plotCellX - playerCellX) / 1f;
            float dy = (plotCellY - playerCellY) / 1f;

            if (Mathf.Abs(dx) <= half + 0.1f && Mathf.Abs(dy) <= half + 0.1f)
            {
                result.Add(plot);
            }
        }

        return result;
    }

    private void PlantInRange()
    {
        if (TimeManager.Instance != null && !TimeManager.Instance.IsDay)
        {
            if (ToastManager.Instance != null)
            {
                ToastManager.Instance.Show("夜晚不能种植");
            }

            return;
        }

        if (seedSelector == null)
        {
            Debug.LogWarning("seedSelector is null");
            return;
        }

        CropData seed = seedSelector.CurrentSeed;
        if (seed == null)
        {
            Debug.LogWarning("CurrentSeed is null");
            return;
        }

        List<FarmPlot> plots = GetPlotsInRange();
        Debug.Log($"PlantInRange: found {plots.Count} plots");
        bool isEggplant = eggplantData != null && seed == eggplantData;
        int eggplantAvailable = GameManager.Instance != null
            ? GameManager.Instance.eggplantSeedCount
            : 0;

        foreach (FarmPlot plot in plots)
        {
            if (plot == null || plot.State != FarmPlot.PlotState.Empty)
            {
                continue;
            }

            if (isEggplant)
            {
                if (eggplantAvailable <= 0)
                {
                    if (ToastManager.Instance != null)
                    {
                        ToastManager.Instance.Show("茄子种子不足");
                    }

                    break;
                }

                eggplantAvailable--;
                GameManager.Instance.SpendEggplantSeed(1);
            }

            plot.PlantCrop(seed);
        }
    }

    private void HarvestInRange()
    {
        if (TimeManager.Instance != null && !TimeManager.Instance.IsDay)
        {
            return;
        }

        List<FarmPlot> plots = GetPlotsInRange();
        foreach (FarmPlot plot in plots)
        {
            if (plot != null && plot.State == FarmPlot.PlotState.Ready)
            {
                plot.Harvest();
            }
        }
    }

    private void UpdateHighlight()
    {
        if (plotHighlight == null || mainCamera == null || playerController == null)
        {
            if (plotHighlight != null)
            {
                plotHighlight.gameObject.SetActive(false);
            }

            return;
        }

        if (TimeManager.Instance == null || !TimeManager.Instance.IsDay)
        {
            plotHighlight.gameObject.SetActive(false);
            return;
        }

        List<FarmPlot> plotsInRange = GetPlotsInRange();
        if (plotsInRange.Count == 0)
        {
            plotHighlight.gameObject.SetActive(false);
            return;
        }

        plotHighlight.gameObject.SetActive(true);

        int rangeSize = playerController.GetFarmRangeSize();

        float cellX = QuantizeToCellCenter(transform.position.x);
        float cellY = QuantizeToCellCenter(transform.position.y);
        Vector3 worldCenter = new Vector3(cellX, cellY, 0f);

        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldCenter);
        screenPos.z = 0f;
        plotHighlight.position = screenPos;

        // 用相邻地块的屏幕距离计算单个格子的像素宽度。
        Vector3 neighborWorld = new Vector3(cellX + 1f, cellY, 0f);
        Vector3 neighborScreen = mainCamera.WorldToScreenPoint(neighborWorld);
        float cellSizePixels = Mathf.Abs(neighborScreen.x - screenPos.x);

        float fullSizePixels = cellSizePixels * rangeSize;

        plotHighlight.sizeDelta = new Vector2(fullSizePixels, fullSizePixels);
    }
}
