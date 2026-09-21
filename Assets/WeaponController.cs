using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public WeaponData[] allWeapons;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public GameObject rocketPrefab;
    public Camera mainCamera;
    public Transform weaponPivot;
    public SpriteRenderer playerSprite;
    public SpriteRenderer weaponSprite;
    public SpriteRenderer weaponSpriteRenderer;
    public CrosshairUI crosshairUI;
    public ParticleSystem muzzleFlash;

    private int currentWeaponIndex = 0;
    private float nextFireTime = 0f;
    private bool isReloading = false;
    private Coroutine reloadCoroutine;

    public int CurrentWeaponIndex => currentWeaponIndex;

    private void Start()
    {
        SelectWeapon(0);
    }

    public void SelectWeapon(int index)
    {
        if (index < 0 || index >= allWeapons.Length)
        {
            return;
        }

        if (!GameManager.Instance.IsWeaponUnlocked(allWeapons[index].weaponName))
        {
            return;
        }

        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
            isReloading = false;
        }

        currentWeaponIndex = index;
        if (weaponSpriteRenderer != null && allWeapons[index].weaponSprite != null)
        {
            weaponSpriteRenderer.sprite = allWeapons[index].weaponSprite;
        }

        crosshairUI.SetCrosshairType(GetCrosshairType(index));
    }

    private void Update()
    {
        bool isUnlocked = GameManager.Instance != null &&
            allWeapons != null && currentWeaponIndex >= 0 && currentWeaponIndex < allWeapons.Length &&
            allWeapons[currentWeaponIndex] != null &&
            GameManager.Instance.IsWeaponUnlocked(allWeapons[currentWeaponIndex].weaponName);

        if (!isUnlocked || HotbarSelector.Instance == null || !HotbarSelector.Instance.IsWeaponSelected)
        {
            if (weaponPivot != null)
            {
                weaponPivot.gameObject.SetActive(false);
            }

            return;
        }

        if (weaponPivot != null)
        {
            weaponPivot.gameObject.SetActive(true);

            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = weaponPivot.position.z;
            Vector2 lookDir = ((Vector2)mouseWorld - (Vector2)weaponPivot.position).normalized;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            weaponPivot.rotation = Quaternion.Euler(0f, 0f, angle);

            if (playerSprite != null)
            {
                playerSprite.flipX = lookDir.x < 0f;
            }

            if (weaponSpriteRenderer != null)
            {
                weaponSpriteRenderer.flipY = lookDir.x < 0f;
            }
            else if (weaponSprite != null)
            {
                weaponSprite.flipY = lookDir.x < 0f;
            }
        }

        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (isReloading)
        {
            return;
        }

        WeaponData weapon = allWeapons[currentWeaponIndex];
        bool shouldFire = weapon.isAutomatic
            ? Input.GetMouseButton(0)
            : Input.GetMouseButtonDown(0);

        if (shouldFire && Time.time >= nextFireTime)
        {
            Fire(weapon);
            nextFireTime = Time.time + weapon.fireRate;
        }
    }

    private void Fire(WeaponData weapon)
    {
        if (AudioManager.Instance != null)
        {
            AudioClip clip = AudioManager.Instance.GetShootSound(weapon.weaponName);
            AudioManager.Instance.PlaySFX(clip);
        }

        if (muzzleFlash != null)
        {
            Debug.Log("Muzzle flash play");
            muzzleFlash.Play();
        }

        crosshairUI.PlayEnlarge();
        Vector2 direction = weaponPivot.right;

        for (int i = 0; i < weapon.bulletCount; i++)
        {
            Quaternion spreadRotation = Quaternion.Euler(
                0f,
                0f,
                Random.Range(-weapon.spread / 2f, weapon.spread / 2f));
            Vector2 rotatedDir = spreadRotation * direction;

            GameObject prefabToUse = bulletPrefab;
            if (weapon.isExplosive && rocketPrefab != null)
            {
                prefabToUse = rocketPrefab;
            }

            GameObject bullet = Instantiate(prefabToUse, firePoint.position, Quaternion.identity);
            Bullet b = bullet.GetComponent<Bullet>();
            if (b != null)
            {
                b.damage = weapon.damage;
                b.isExplosive = weapon.isExplosive;
                b.explosionRadius = weapon.explosionRadius;
            }

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = rotatedDir * weapon.bulletSpeed;
            }

            float bulletAngle = Mathf.Atan2(rotatedDir.y, rotatedDir.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, bulletAngle);
        }

        if (weapon.weaponSpriteEmpty != null)
        {
            if (reloadCoroutine != null)
            {
                StopCoroutine(reloadCoroutine);
            }

            reloadCoroutine = StartCoroutine(ReloadRoutine(weapon));
        }
    }

    private IEnumerator ReloadRoutine(WeaponData weapon)
    {
        isReloading = true;

        if (weaponSpriteRenderer != null && weapon.weaponSpriteEmpty != null)
        {
            weaponSpriteRenderer.sprite = weapon.weaponSpriteEmpty;
        }

        yield return new WaitForSeconds(weapon.fireRate);

        if (weaponSpriteRenderer != null && allWeapons[currentWeaponIndex] == weapon)
        {
            weaponSpriteRenderer.sprite = weapon.weaponSprite;
        }

        isReloading = false;
        reloadCoroutine = null;
    }

    private int GetCrosshairType(int weaponIndex)
    {
        // 0 = 手枪, 1 = 霰弹枪, 2 = 步枪, 3 = 火箭筒
        if (weaponIndex == 1)
        {
            return 1;
        }

        if (weaponIndex == 3)
        {
            return 2;
        }

        return 0;
    }
}
