using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField] float seconds;

    private void Awake()
    {
        Destroy(gameObject, seconds);
    }
}
