using UnityEngine;

public class gosteavego : MonoBehaviour
{
    [Header("移动")]
    public float moveSpeed = 5f;
    public int moveSpeedLevel = 0;
    public int maxMoveSpeedLevel = 3;
    public float moveSpeedPerLevel = 0.5f;
    public int moveSpeedUpgradeCost = 150;

    [Header("种植范围")]
    public int farmRangeLevel = 0;
    public int maxFarmRangeLevel = 1;
    public int farmRangeUpgradeCost = 200;

    private Animator ani;

    private void Start()
    {
        ani = GetComponent<Animator>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
        {
            ani.SetFloat("Horizontal", horizontal);
            ani.SetFloat("Vertical", 0);
        }

        if (vertical != 0)
        {
            ani.SetFloat("Horizontal", 0);
            ani.SetFloat("Vertical", vertical);
        }

        Vector2 direction = new Vector2(horizontal, vertical);
        ani.SetFloat("Speed", direction.magnitude);
        Move(direction);
    }

    private void Move(Vector2 direction)
    {
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    public int GetFarmRangeSize()
    {
        return 1 + farmRangeLevel * 2;
    }

    public bool UpgradeMoveSpeed()
    {
        if (moveSpeedLevel >= maxMoveSpeedLevel)
        {
            return false;
        }

        if (GameManager.Instance == null || !GameManager.Instance.SpendCoins(moveSpeedUpgradeCost))
        {
            return false;
        }

        moveSpeedLevel++;
        moveSpeed += moveSpeedPerLevel;
        moveSpeedUpgradeCost += 100;
        return true;
    }

    public bool UpgradeFarmRange()
    {
        if (farmRangeLevel >= maxFarmRangeLevel)
        {
            return false;
        }

        if (GameManager.Instance == null || !GameManager.Instance.SpendCoins(farmRangeUpgradeCost))
        {
            return false;
        }

        farmRangeLevel++;
        farmRangeUpgradeCost += 150;
        return true;
    }
}
