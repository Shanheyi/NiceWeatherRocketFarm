using UnityEngine;

public class UpgradeTerminal : MonoBehaviour
{
    public GameObject bubbleCanvas;
    public UpgradeShopUI shopUI;
    public KeyCode openKey = KeyCode.F;

    private bool playerInRange = false;

    private void Start()
    {
        if (bubbleCanvas != null)
        {
            bubbleCanvas.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(openKey))
        {
            shopUI.Toggle();
        }
    }
}
