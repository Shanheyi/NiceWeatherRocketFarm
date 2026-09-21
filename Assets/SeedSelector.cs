using UnityEngine;

public class SeedSelector : MonoBehaviour
{
    public CropData[] availableSeeds;

    private int currentIndex = 0;

    public CropData CurrentSeed => availableSeeds.Length > 0 ? availableSeeds[currentIndex] : null;

    public void SelectSeed(int index)
    {
        if (index < 0 || index >= availableSeeds.Length)
        {
            return;
        }

        currentIndex = index;
    }
}
