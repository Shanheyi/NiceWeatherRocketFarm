using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    public Image crosshairImage;
    public GameObject crosshairObject;
    public GameObject normalCursorObject;
    public GameObject shopPanel;
    public GameObject startPanel;
    public Sprite pistolNormal;
    public Sprite pistolEnlarged;
    public Sprite shotgunNormal;
    public Sprite shotgunEnlarged;
    public Sprite sniperNormal;
    public Sprite sniperEnlarged;
    public float enlargedDuration = 0.1f;

    private Coroutine enlargeCoroutine;
    private Sprite currentNormal;
    private Sprite currentEnlarged;

    private void Start()
    {
        Cursor.visible = false;
        crosshairImage.raycastTarget = false;
        SetCrosshairType(0);
        ShowNormalCursor();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded)
        {
            SetCrosshairVisible(false);

            if (normalCursorObject != null)
            {
                normalCursorObject.SetActive(false);
            }

            return;
        }

        if (startPanel != null && startPanel.activeInHierarchy)
        {
            SetCrosshairVisible(false);

            if (normalCursorObject != null)
            {
                normalCursorObject.SetActive(false);
            }

            return;
        }

        if (shopPanel != null && shopPanel.activeInHierarchy)
        {
            SetCrosshairVisible(false);

            if (normalCursorObject != null)
            {
                normalCursorObject.SetActive(true);
                normalCursorObject.transform.position = Input.mousePosition;
            }

            return;
        }

        bool isWeaponSelected = HotbarSelector.Instance != null && HotbarSelector.Instance.IsWeaponSelected;
        bool showCrosshair = isWeaponSelected;

        SetCrosshairVisible(showCrosshair);

        if (crosshairObject != null)
        {
            crosshairObject.GetComponent<RectTransform>().position = Input.mousePosition;
        }

        if (normalCursorObject != null)
        {
            normalCursorObject.SetActive(!showCrosshair);
            normalCursorObject.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }

    public void ShowNormalCursor()
    {
        SetCrosshairVisible(false);

        if (normalCursorObject != null)
        {
            normalCursorObject.SetActive(true);
        }
    }

    private void SetCrosshairVisible(bool isVisible)
    {
        if (crosshairObject == null)
        {
            return;
        }

        bool wouldDisableThisScript = crosshairObject == gameObject || transform.IsChildOf(crosshairObject.transform);
        if (wouldDisableThisScript)
        {
            crosshairImage.enabled = isVisible;
        }
        else
        {
            crosshairObject.SetActive(isVisible);
        }
    }

    public void SetCrosshairType(int type)
    {
        switch (type)
        {
            case 0:
                currentNormal = pistolNormal;
                currentEnlarged = pistolEnlarged;
                break;
            case 1:
                currentNormal = shotgunNormal;
                currentEnlarged = shotgunEnlarged;
                break;
            case 2:
                currentNormal = sniperNormal;
                currentEnlarged = sniperEnlarged;
                break;
        }

        crosshairImage.sprite = currentNormal;
    }

    public void PlayEnlarge()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        if (currentEnlarged == null)
        {
            return;
        }

        if (enlargeCoroutine != null)
        {
            StopCoroutine(enlargeCoroutine);
        }

        enlargeCoroutine = StartCoroutine(EnlargeRoutine());
    }

    private IEnumerator EnlargeRoutine()
    {
        crosshairImage.sprite = currentEnlarged;
        yield return new WaitForSecondsRealtime(enlargedDuration);
        crosshairImage.sprite = currentNormal;
        enlargeCoroutine = null;
    }

    private void OnDestroy()
    {
        Cursor.visible = true;
    }
}
