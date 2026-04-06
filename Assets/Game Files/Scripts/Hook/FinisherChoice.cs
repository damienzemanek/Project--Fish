using System;
using System.Collections;
using EMILtools.Systems;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Splines;

public class FinisherChoice :
    MonoBehaviour,
    IBoundsCheckMsgReceiver<Collider2D, HookFinisherBoundsChecker.HookFinisherContext>
{
    [Required] public SplineAnimate splineAnimate;
    public FinisherEvent events;
    WaitForSeconds waitTime;

    void Awake()
    {
        waitTime = new WaitForSeconds(splineAnimate.Duration);
    }
    
    public void PlayEvent() => StartCoroutine(C_PlayEvent());
    
    IEnumerator C_PlayEvent()
    {
        splineAnimate.Restart(autoplay: true);
        yield return waitTime;
        events.OnExit();
    }
    
    // /// <summary>
    // /// When Finisher Spline is in Finisher Bounds
    // /// </summary>
    // /// <param name="collidedWith"></param>
    // /// <param name="sender"></param>
    // /// <param name="ctx"></param>
    // public void OnEnterBounds(Collider2D collidedWith, BoundsChecker<HookFinisherBoundsChecker.HookFinisherContext> sender, HookFinisherBoundsChecker.HookFinisherContext ctx) 
    //     => receiver.Send("FINISHER", true);
    //
    // /// <summary>
    // /// When Finisher Spline is out of Finisher Bounds
    // /// </summary>
    // /// <param name="collidedWith"></param>
    // /// <param name="sender"></param>
    // /// <param name="ctx"></param>
    // public void OnExitBounds(Collider2D collidedWith, BoundsChecker<HookFinisherBoundsChecker.HookFinisherContext> sender, HookFinisherBoundsChecker.HookFinisherContext ctx) 
    //     => receiver.Send("FINISHER", false);
    

}
