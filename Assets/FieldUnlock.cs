using UnityEngine;

public class FieldUnlock : MonoBehaviour
{
    public FarmField targetField;
    public int unlockCost = 200;
    public KeyCode unlockKey = KeyCode.U;
    public GameObject lockedVisual;
    public GameObject unlockPanel;
    public TMPro.TextMeshProUGUI unlockHintText;

    private bool playerInRange = false;

    private void Start()
    {
        if (unlockPanel != null)
        {
            unlockPanel.SetActive(false);
        }

        if (unlockHintText != null)
        {
            unlockHintText.text = $"按 U 解锁({unlockCost} 金币)";
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerInRange)
            {
                return;
            }

            playerInRange = true;
            UpdateUnlockPanel();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            UpdateUnlockPanel();
        }
    }

    private void Update()
    {
        if (playerInRange && !targetField.IsUnlocked && Input.GetKeyDown(unlockKey))
        {
            if (GameManager.Instance.SpendCoins(unlockCost))
            {
                targetField.Unlock();
                lockedVisual?.SetActive(false);
                UpdateUnlockPanel();

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.unlockSound);
                }
            }
            else
            {
                ToastManager.Instance.Show("金币不足");
            }
        }
    }

    private void UpdateUnlockPanel()
    {
        if (unlockPanel != null)
        {
            unlockPanel.SetActive(playerInRange && !targetField.IsUnlocked);
        }
    }
}
