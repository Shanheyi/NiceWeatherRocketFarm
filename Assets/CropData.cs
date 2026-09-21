using UnityEngine;

[CreateAssetMenu(fileName = "NewCrop", menuName = "Farm/Crop Data")]
public class CropData : ScriptableObject
{
    public string cropName;
    public Sprite[] growthStages;
    public float timePerStage = 5f;
    public int sellPrice = 10;
}
