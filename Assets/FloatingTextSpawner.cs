using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    public static FloatingTextSpawner Instance;

    public GameObject floatingTextPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void Spawn(string text, Vector3 worldPosition)
    {
        GameObject go = Instantiate(floatingTextPrefab, worldPosition, Quaternion.identity);
        go.GetComponent<FloatingText>().Setup(text);
    }
}
