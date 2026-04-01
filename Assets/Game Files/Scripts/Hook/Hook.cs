using System;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public LayerMask worldMask;
    public LayerMask targetMask;
    public Rigidbody2D rb;
    
    public Transform rodParent;

    Action attachedSignal;

    private void Start()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    bool InMask(Collision other, LayerMask mask) => (mask.value & (1 << other.gameObject.layer)) != 0;
    
    private void OnCollisionEnter(Collision other)
    {
        if(InMask(other, worldMask)) StopHookMovement();
        if(InMask(other, targetMask)) AttachHook(other.transform);
    }

    public void CastHook(Vector2 dirWithForce)
    {
        Rb2DEX.ResetVel2D(rb);
        rb.AddForce(dirWithForce, ForceMode2D.Impulse);
        rb.bodyType = RigidbodyType2D.Dynamic;
        transform.SetParent(null);
    }

    public void ResetHook()
    {
        transform.SetParent(rodParent);
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    void StopHookMovement()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void AttachHook(Transform parent)
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.SetParent(parent);
        attachedSignal?.Invoke();
    }
    
    public void InjectAttachedSignal(Action signal) => attachedSignal = signal;
}
