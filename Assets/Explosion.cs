using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float lifetime = 0.7f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
