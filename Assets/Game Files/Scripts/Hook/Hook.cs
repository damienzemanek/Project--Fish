using System;
using System.Collections;
using System.Collections.Generic;
using EMILtools.Timers;
using UnityEngine;

public class Hook : MonoBehaviour
{
    private bool isHooking;
    public LayerMask worldMask;
    public LayerMask targetMask;
    public Rigidbody2D rb;
    
    public Transform rodParent;
    public Collider2D hookCollider;
    public DistanceJoint2D joint;

    public AnimationCurve lineCurve;
    public LineRenderer line;

    public float maxDistanceToAutoRecal = 8f;
    public int linePoints = 10;
    
    Action attachedSignal;

    void Awake()
    {
        line.positionCount = linePoints;
    }

    private void Start()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        joint.enabled = false;
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, rodParent.position);
        int count = line.positionCount;
        float step = dist / (count - 1);
        Vector3 dir = (transform.position - rodParent.position);
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = line.transform.InverseTransformPoint(line.transform.position + dir * (step * i));
            pos = pos / dist;
            line.SetPosition(i, pos);   
        }
    }

    bool InMask(Collision2D other, LayerMask mask) => (mask.value & (1 << other.gameObject.layer)) != 0;
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(InMask(other, worldMask)) StopHookMovement();
        if(InMask(other, targetMask)) AttachHook(other.transform);
    }

    public void CastHook(Vector2 dirWithForce)
    {
        hookCollider.enabled = true;
        joint.enabled = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.ResetVel2D();
        rb.AddForce(dirWithForce, ForceMode2D.Impulse);
        transform.position = rodParent.position;
        transform.SetParent(null);
        isHooking = true;
        StartCoroutine(C_CheckIfHookDistanceIsTooFar());
        
    }

    public void ResetHook()
    {
        transform.SetParent(rodParent);
        transform.position = rodParent.position;
        rb.bodyType = RigidbodyType2D.Kinematic;
        joint.enabled = false;
    }

    void StopHookMovement()
    {
        Debug.Log("HIT GROUND");
        rb.bodyType = RigidbodyType2D.Kinematic;
        hookCollider.enabled = false;
        rb.ResetVel2D();
    }

    void AttachHook(Transform parent)
    {
        Debug.Log("HOOK ATTACHING");
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.SetParent(parent);
        rb.ResetVel2D();
        attachedSignal?.Invoke();
        hookCollider.enabled = false;
    }

    void HookReachedMaxTime()
    {
        joint.enabled = true;
        joint.distance = Vector3.Distance(transform.position, rodParent.position);
    }

    IEnumerator C_CheckIfHookDistanceIsTooFar()
    {
        while (isHooking)
        {
            yield return null;
            float dist = Vector3.Distance(transform.position, rodParent.position);
            if (dist > maxDistanceToAutoRecal)
            {
                HookReachedMaxTime();
                yield break;
            }
                
        }
    }

    void RecallHook()
    {
        Debug.Log("HOOK RECALLED");
    }
    
    public void InjectAttachedSignal(Action signal) => attachedSignal = signal;
}
