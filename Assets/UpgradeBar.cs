using UnityEngine;
using UnityEngine.UI;

public class UpgradeBar : MonoBehaviour
{
    public Image[] slots;
    public Sprite emptySlotSprite;
    public Sprite filledSlotSprite;

    public void SetLevel(int level)
    {
        if (slots == null)
        {
            return;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                continue;
            }

            slots[i].sprite = i < level ? filledSlotSprite : emptySlotSprite;
        }
    }
}
