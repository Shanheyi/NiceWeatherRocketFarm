using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;
    public float lifetime = 3f;
    public GameObject hitEffect;
    public bool isExplosive = false;
    public float explosionRadius = 1.5f;
    public GameObject explosionEffectPrefab;
    public AudioClip explosionSound;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (isExplosive)
            {
                TriggerExplosion();
            }
            else
            {
                other.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            }

            Destroy(gameObject);
        }
    }

    private void TriggerExplosion()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            }
        }

        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        if (AudioManager.Instance != null && explosionSound != null)
        {
            AudioManager.Instance.PlaySFX(explosionSound);
        }
    }
}
