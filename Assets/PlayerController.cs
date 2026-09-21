using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int moveSpeedLevel = 0;
    public int maxMoveSpeedLevel = 5;
    public float moveSpeedPerLevel = 0.5f;
    public int moveSpeedUpgradeCost = 150;
    public int farmRangeLevel = 0;
    public int maxFarmRangeLevel = 4;
    public int farmRangeUpgradeCost = 200;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical"));

        Vector3 direction = input.normalized;
        transform.position += direction * (moveSpeed * Time.deltaTime);
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

    public int GetFarmRangeSize()
    {
        return farmRangeLevel + 1;
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
