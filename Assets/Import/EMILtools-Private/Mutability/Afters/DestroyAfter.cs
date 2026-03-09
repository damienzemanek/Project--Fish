using UnityEngine;

/// <summary>
/// Simple alternative to object pooling
/// For Quick Prototyping
/// </summary>
public class DestroyAfter : MonoBehaviour
{
    public float time;
    void Start() => Destroy(gameObject, time);
}
