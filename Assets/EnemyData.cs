using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Farm/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int maxHealth = 30;
    public int damage = 10;
    public float moveSpeed = 1f;
    public float attackInterval = 1f;
    public float attackRange = 0.5f;
    public Sprite[] moveFrames;
    public Sprite[] attackFrames;
    public Sprite[] deathFrames;
    public float animationFrameDuration = 0.1f;
    public int spawnTier = 1;
}
