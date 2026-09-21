using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData data;
    public SpriteRenderer spriteRenderer;
    public Transform fenceTarget;
    public float flashDuration = 0.1f;
    public Transform healthBarRoot;
    public Transform healthBarFill;
    public float maxBarWidth = 1f;
    public float eggplantDropChance = 0.5f;

    private int currentHealth;
    private float nextAttackTime = 0f;
    private bool isDead = false;
    private bool isAttacking = false;
    private Sprite[] currentFrames;
    private int currentFrame = 0;
    private float frameTimer = 0f;
    private Coroutine flashCoroutine;
    private Color originalColor;

    private void Start()
    {
        originalColor = spriteRenderer.color;
        currentHealth = data.maxHealth;
        currentFrames = data.moveFrames;
        if (healthBarRoot != null)
        {
            healthBarRoot.gameObject.SetActive(true);
        }

        UpdateHealthBar();
        DayManager.Instance.RegisterEnemy();

        if (fenceTarget == null)
        {
            fenceTarget = GameObject.Find("Fence").transform;
        }
    }

    private void Update()
    {
        if (isDead)
        {
            UpdateAnimation();
            return;
        }

        if (isAttacking)
        {
            currentFrames = data.attackFrames;
            if (Time.time >= nextAttackTime)
            {
                Fence.Instance.TakeDamage(data.damage);
                nextAttackTime = Time.time + data.attackInterval;
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                fenceTarget.position,
                data.moveSpeed * Time.deltaTime);
            currentFrames = data.moveFrames;
        }

        UpdateAnimation();
        spriteRenderer.flipX = fenceTarget.position.x > transform.position.x;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fence"))
        {
            isAttacking = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Fence"))
        {
            isAttacking = false;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= amount;
        UpdateHealthBar();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyHit);
        }

        PlayFlash();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        if (Random.value < eggplantDropChance)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddEggplantSeed(1);
            }

            if (FloatingTextSpawner.Instance != null)
            {
                Vector3 pos = transform.position + Vector3.up * 1f;
                FloatingTextSpawner.Instance.Spawn("+1 茄子种子", pos);
            }
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyDie);
        }

        if (healthBarRoot != null)
        {
            healthBarRoot.gameObject.SetActive(false);
        }

        DayManager.Instance.EnemyDefeated();
        currentFrames = data.deathFrames;
        currentFrame = 0;
        frameTimer = 0f;
        spriteRenderer.sprite = currentFrames[currentFrame];
        Destroy(gameObject, data.deathFrames.Length * data.animationFrameDuration);
    }

    public void PlayFlash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
        flashCoroutine = null;
    }

    private void UpdateAnimation()
    {
        if (currentFrames == null || currentFrames.Length == 0)
        {
            return;
        }

        frameTimer += Time.deltaTime;
        if (frameTimer >= data.animationFrameDuration)
        {
            currentFrame = (currentFrame + 1) % currentFrames.Length;
            spriteRenderer.sprite = currentFrames[currentFrame];
            frameTimer = 0f;
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill == null || data == null)
        {
            return;
        }

        float ratio = Mathf.Clamp01((float)currentHealth / data.maxHealth);
        Vector3 scale = healthBarFill.localScale;
        scale.x = maxBarWidth * ratio;
        healthBarFill.localScale = scale;
    }
}
