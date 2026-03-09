using System;
using EMILtools.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using static EMILtools.Extensions.PhysEX;

[Serializable]
public class AugmentPhysEX : MonoBehaviour
{
    public JumpSettings jumpSettings;
    public GroundedSettings groundedSettings;
    public FallSettings fallSettings;

    public bool fallFaster = true;

    [BoxGroup("Rigidbody")] [SerializeField] Rigidbody rb;
    [BoxGroup("ReadOnly")] [ReadOnly] public ReactiveIntercept<bool> isGrounded;

    void FixedUpdate()
    {
        isGrounded.Value = transform.IsGrounded(ref groundedSettings);
        if(!isGrounded && fallFaster) rb.FallFaster(fallSettings);
    }
}
