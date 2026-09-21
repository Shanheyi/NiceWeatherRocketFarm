using System.Collections;
using UnityEngine;

public class FarmPlot : MonoBehaviour
{
    public enum PlotState { Empty, Growing, Ready }

    public SpriteRenderer cropVisual;
    public Sprite emptySprite;
    public FarmField ownerField;
    public int plotIndex;

    private PlotState state = PlotState.Empty;
    private int currentStage = 0;
    private Coroutine growCoroutine;
    private CropData currentCrop;

    public PlotState State => state;
    public bool CanInteract => state != PlotState.Growing;

    public void Interact(CropData seedToPlant)
    {
        if (TimeManager.Instance != null && !TimeManager.Instance.IsDay)
        {
            ToastManager.Instance.Show("夜晚不能种植");
            return;
        }

        if (ownerField != null && !ownerField.IsUnlocked)
        {
            return;
        }

        if (state == PlotState.Empty)
        {
            if (seedToPlant != null)
            {
                PlantCrop(seedToPlant);
            }
        }
        else if (state == PlotState.Ready)
        {
            Harvest();
        }
    }

    public void PlantCrop(CropData crop)
    {
        if (crop == null)
        {
            return;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.plantSound);
        }

        currentCrop = crop;
        currentStage = 0;

        if (cropVisual != null)
        {
            cropVisual.sprite = crop.growthStages[0];
        }

        state = PlotState.Growing;
        growCoroutine = StartCoroutine(GrowRoutine());
    }

    private IEnumerator GrowRoutine()
    {
        while (currentStage < currentCrop.growthStages.Length - 1)
        {
            while (!TimeManager.Instance.IsDay)
            {
                yield return null;
            }

            yield return new WaitForSeconds(currentCrop.timePerStage);
            currentStage++;

            if (cropVisual != null)
            {
                cropVisual.sprite = currentCrop.growthStages[currentStage];
            }
        }

        state = PlotState.Ready;
        growCoroutine = null;
    }

    public void Harvest()
    {
        if (currentCrop == null)
        {
            return;
        }

        int price = currentCrop.sellPrice;
        GameManager.Instance.AddCoins(price);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.harvestSound);
        }

        if (FloatingTextSpawner.Instance != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.8f;
            FloatingTextSpawner.Instance.Spawn($"+{price}", spawnPos);
        }

        state = PlotState.Empty;

        if (cropVisual != null)
        {
            cropVisual.sprite = emptySprite;
        }

        currentCrop = null;
        currentStage = 0;
    }

    private void OnDestroy()
    {
        if (growCoroutine != null)
        {
            StopCoroutine(growCoroutine);
        }
    }
}
