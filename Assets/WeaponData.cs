using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Farm/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public string displayName;
    public int damage = 10;
    public float fireRate = 0.5f;
    public float bulletSpeed = 10f;
    public int bulletCount = 1;
    public float spread = 0f;
    public bool isAutomatic = false;
    public bool isExplosive = false;
    public float explosionRadius = 1.5f;
    public int unlockCost = 0;
    public Sprite weaponSprite;
    public Sprite weaponSpriteEmpty;
    public Sprite lockedIcon;
    public Sprite unlockedIcon;
}
