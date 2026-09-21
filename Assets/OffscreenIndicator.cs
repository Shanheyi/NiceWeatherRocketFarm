using UnityEngine;
using UnityEngine.UI;

public class OffscreenIndicator : MonoBehaviour
{
    public Image[] arrowPool;
    public Camera mainCamera;
    public float edgePadding = 50f;

    private void Update()
    {
        if (arrowPool == null)
        {
            return;
        }

        foreach (Image arrow in arrowPool)
        {
            if (arrow != null)
            {
                arrow.gameObject.SetActive(false);
            }
        }

        if (mainCamera == null)
        {
            return;
        }

        int arrowIndex = 0;
        int maxArrows = Mathf.Min(8, arrowPool.Length);
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (arrowIndex >= maxArrows)
            {
                break;
            }

            Vector3 screenPosition = mainCamera.WorldToScreenPoint(enemy.transform.position);
            bool isOffscreen = screenPosition.z < 0f || screenPosition.x < 0f || screenPosition.x > Screen.width ||
                               screenPosition.y < 0f || screenPosition.y > Screen.height;

            if (!isOffscreen)
            {
                continue;
            }

            Image arrow = arrowPool[arrowIndex];
            arrowIndex++;
            if (arrow == null)
            {
                continue;
            }

            arrow.gameObject.SetActive(true);
            arrow.rectTransform.position = GetScreenEdgePosition(enemy.transform.position);

            Vector3 enemyScreenPosition = mainCamera.WorldToScreenPoint(enemy.transform.position);
            if (enemyScreenPosition.z < 0f)
            {
                enemyScreenPosition.x = Screen.width - enemyScreenPosition.x;
                enemyScreenPosition.y = Screen.height - enemyScreenPosition.y;
            }

            Vector2 direction = ((Vector2)enemyScreenPosition - new Vector2(Screen.width / 2f, Screen.height / 2f)).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrow.rectTransform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }

    public Vector2 GetScreenEdgePosition(Vector2 worldPos)
    {
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPos);
        if (screenPosition.z < 0f)
        {
            screenPosition.x = Screen.width - screenPosition.x;
            screenPosition.y = Screen.height - screenPosition.y;
        }

        return new Vector2(
            Mathf.Clamp(screenPosition.x, edgePadding, Screen.width - edgePadding),
            Mathf.Clamp(screenPosition.y, edgePadding, Screen.height - edgePadding));
    }
}
