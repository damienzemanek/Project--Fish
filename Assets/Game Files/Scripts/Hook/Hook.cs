using System;
using System.Collections;
using System.Collections.Generic;
using EMILtools.Extensions;
using EMILtools.Timers;
using UnityEngine;

public class Hook : MonoBehaviour, TimerUtility.ITimerUser
{
    public enum HookPhases
    {
        CastingOut,
        HitMaxDistAndFalling,
        HookedToSomething,
        Drooping,
        Recalling
    }
    public bool withinRange;
    public HookPhases phase;
    public LayerMask worldMask;
    public LayerMask targetMask;
    public Rigidbody2D rb;
    
    public Transform rodParent;
    public Collider2D hookCollider;
    public DistanceJoint2D joint;

    public AnimationCurve lineVerticalOffsetCurve;
    public float verticalOffsetScalar = 3f;
    public LineRenderer line;

    public float maxDistanceToAutoRecal = 8f;
    public int linePoints = 10;
    public float lineLerpSpeed = 10f;
    public float dist;
    public Ref<float> remainingCurveWhenAtMaxDist = 0.5f;

    public CountdownTimer lineDroopTimer;
    public Ref<float> lineDroopTime = 0.5f;
    
    public CountdownTimer snapToHookedTimer;
    public Ref<float> snapTimeWhenHooked = 0.2f;
    
    Action attachedSignal;

    void Awake()
    {
        line.positionCount = linePoints;
        lineDroopTimer = new CountdownTimer(lineDroopTime, true);
        snapToHookedTimer = new CountdownTimer(snapTimeWhenHooked, true);
        this.InitTimer(lineDroopTimer, true);
        this.InitTimer(snapToHookedTimer, true);
    }

    private void Start()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        joint.enabled = false;
    }

    void Update()
    {
        dist = Vector3.Distance(transform.position, rodParent.position);
        withinRange = dist < maxDistanceToAutoRecal;
        
        int count = line.positionCount;
        float step = dist / (count - 1);
        Vector3 dir = (transform.position - rodParent.position);
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = (line.transform.InverseTransformPoint(line.transform.position + dir * (step * i))) / dist;
            
            
            switch (phase)
            {
                case HookPhases.CastingOut: CastOut(); break;
                case HookPhases.HitMaxDistAndFalling: HitMaxDistAndFalling(); break;
                case HookPhases.HookedToSomething: HookedOntoSomething(); break;
            }


            void CastOut()
            {
                float point = (float)i / (count - 1); 
                float curvedScalar = verticalOffsetScalar * lineVerticalOffsetCurve.Evaluate(point);
                
                float distProg = dist.ProgressWhenApproaching(0f, maxDistanceToAutoRecal, true);
                float distanceResponsiveScalar = curvedScalar * ((1f + remainingCurveWhenAtMaxDist) - distProg);

                // Dont apply offset to end points
                float nonEndPointsOffset = (i != 0 && i != count - 1) ? distanceResponsiveScalar : 1f;

                
                pos = pos.With(y: pos.y + nonEndPointsOffset);
               // Debug.Log($"Curved: {distanceResponsiveScalar} Regular: {nonEndPointsOffset}, Dist Prog : {distProg}");
                line.SetPosition(i, pos);
            }
            
            void HitMaxDistAndFalling()
            {
                if(!lineDroopTimer.isRunning) lineDroopTimer.StartAndReset();

                float remainingCurveWhenAtMaxDist_Falling = remainingCurveWhenAtMaxDist * (lineDroopTimer.Progress);
                
                float point = (float)i / (count - 1); 
                float curvedScalar = verticalOffsetScalar * lineVerticalOffsetCurve.Evaluate(point);
                
                float distProg = dist.ProgressWhenApproaching(0f, maxDistanceToAutoRecal, true);
                float distanceResponsiveScalar = curvedScalar * ((1f + remainingCurveWhenAtMaxDist_Falling) - distProg);

                // Dont apply offset to end points
                float nonEndPointsOffset = (i != 0 && i != count - 1) ? distanceResponsiveScalar : 1f;

                
                pos = pos.With(y: pos.y + nonEndPointsOffset);
                //Debug.Log($"Curved: {distanceResponsiveScalar} Regular: {nonEndPointsOffset}, Dist Prog : {distProg}");
                line.SetPosition(i, pos);
            }

            void HookedOntoSomething()
            {
                if(!snapToHookedTimer.isRunning) snapToHookedTimer.StartAndReset();
                
                Debug.Log("snap prog : " + (1-snapToHookedTimer.Progress));
                
                Vector2 oldPos = line.GetPosition(i);
                Vector2 newPos = pos;
                Vector2 lerpPos = Vector2.Lerp(oldPos, newPos, (1- snapToHookedTimer.Progress));
                
                line.SetPosition(i, lerpPos); 
            }
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
        phase = HookPhases.CastingOut;
        lineDroopTimer.canRun = true;
        snapToHookedTimer.canRun = true;
        StartCoroutine(C_CheckIfHookDistanceIsTooFar());
        
    }

    public void ResetHook()
    {
        transform.SetParent(rodParent);
        transform.position = rodParent.position;
        rb.bodyType = RigidbodyType2D.Kinematic;
        joint.enabled = false;
        phase = HookPhases.Recalling;
        withinRange = false;
    }

    void StopHookMovement()
    {
        Debug.Log("HIT GROUND");
        rb.bodyType = RigidbodyType2D.Kinematic;
        hookCollider.enabled = false;
        rb.ResetVel2D(); 
        phase = HookPhases.HookedToSomething;
        if(!lineDroopTimer.isRunning && phase != HookPhases.Drooping) lineDroopTimer.StartAndReset();
    }

    void AttachHook(Transform parent)
    {
        Debug.Log("HOOK ATTACHING");
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.SetParent(parent);
        rb.ResetVel2D();
        attachedSignal?.Invoke();
        hookCollider.enabled = false;
        phase = HookPhases.HookedToSomething;
        if(!lineDroopTimer.isRunning && phase != HookPhases.Drooping) lineDroopTimer.StartAndReset();
    }

    void HookReachedMaxDist()
    {
        joint.enabled = true;
        phase = HookPhases.HitMaxDistAndFalling;
        joint.distance = Vector3.Distance(transform.position, rodParent.position);
    }

    IEnumerator C_CheckIfHookDistanceIsTooFar()
    {
        while (phase == HookPhases.CastingOut)
        {
            yield return null;
            dist = Vector3.Distance(transform.position, rodParent.position);
            if (!(dist > maxDistanceToAutoRecal)) continue;
            HookReachedMaxDist();
            yield break;

        }
    }

    void RecallHook()
    {
        Debug.Log("HOOK RECALLED");
    }
    
    public void InjectAttachedSignal(Action signal) => attachedSignal = signal;
}
