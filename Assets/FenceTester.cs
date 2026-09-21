using UnityEngine;

public class FenceTester : MonoBehaviour
{
    public KeyCode damageKey = KeyCode.H;
    public KeyCode healKey = KeyCode.J;
    public int damageAmount = 10;
    public int healAmount = 20;

    private void Update()
    {
        if (Input.GetKeyDown(damageKey))
        {
            Fence.Instance.TakeDamage(damageAmount);
            Debug.Log($"Fence took {damageAmount} damage. Current: {Fence.Instance.currentHealth}/{Fence.Instance.maxHealth}");
        }

        if (Input.GetKeyDown(healKey))
        {
            Fence.Instance.Heal(healAmount);
            Debug.Log($"Fence healed {healAmount}. Current: {Fence.Instance.currentHealth}/{Fence.Instance.maxHealth}");
        }
    }
}
