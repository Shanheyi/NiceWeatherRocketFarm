using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image targetImage;
    public Sprite normalSprite;
    public Sprite pressedSprite;

    private void Start()
    {
        targetImage.sprite = normalSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetImage.sprite = pressedSprite;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.uiClick);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetImage.sprite = normalSprite;
    }
}
