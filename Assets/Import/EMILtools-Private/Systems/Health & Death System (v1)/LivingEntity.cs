using System;
using System.Collections;
using System.Collections.Generic;
using EMILtools.Core;
using EMILtools.Extensions;
using EMILtools.Systems;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using static EMILtools.Extensions.NumEX;
using static IDamageable;
using static LivingEntity;

public interface ILivingEntityControlled
{
    public PersistentFunc<DamageInfo, float> TakeDamageCaller { get; }
    public PersistentAction<DeathType> OnDeath { set; }
}

public class LivingEntity : Entity,
    IDamageable,
    IHealable<Enum>,
    ILivingEntityControlled
{
    public enum BasicHealthThresholdEnum { Alive, Dying, Dead }
    
    /// <summary>
    /// Not all phases need to be used, for instance you could start at phase three
    /// </summary>
    public enum PhasedHealthThresholdEnum { PhaseFive, PhaseFour, PhaseThree, PhaseTwo, PhaseOne, Dying, Dead }
    
    const float RestartAnimation = ZeroF;
    const float FromBeginning = ZeroF;
    float maxHealth;


    [FoldoutGroup("ReadOnly")] [ShowInInspector] public ReactiveIntercept<float> health;
    [FoldoutGroup("ReadOnly")] [ShowInInspector, ReadOnly] public bool isDead = false;
    [FoldoutGroup("ReadOnly")] [ShowInInspector, ReadOnly] public DeathType deathStatus;
    [FoldoutGroup("ReadOnly")] [ShowInInspector, ReadOnly] public int healthThresholdsIndex;

    
    [FoldoutGroup("Settings")] public ThresholdCore healthThresholds;
    IDelegator allThresholdCb;
    Dictionary<Enum, IDelegator> thresholdCbs = new();

    public void SetAllThresholdCallbacks(IDelegator cb) => allThresholdCb = cb;
    public void AddOrReplaceThresholdCallback(Enum label, IDelegator cb) => thresholdCbs[label] = cb;
    
    [FoldoutGroup("Death")] public bool destroyOnDeath = false;
    [FoldoutGroup("Death")] [ShowIf("destroyOnDeath")] public float destroyOnDeathTime = 2f;
    [FoldoutGroup("Death")] [Required] public Rigidbody2D rb;
    [FoldoutGroup("Death")] [Required] public Collider2D deathFloorCollider;
    [FoldoutGroup("Death")] public List<GameObject> enableOnDeathAndUnparents = new();

    
    [FoldoutGroup("Animation")] [Required] public Animator animator;
    [FoldoutGroup("Animation")] public AnimHandle<DeathType, NoBlends> deathAnimHandle;
    [FoldoutGroup("Animation")] public AnimHandle<DamageLocation, NoBlends> damageLocationAnimHandle;
    public PersistentFunc<DamageInfo, float> TakeDamageCaller { get; private set; }
    public PersistentAction<DeathType> OnDeath { get; set; } = new();
    [FoldoutGroup("Death")] public UnityEvent OnDeathUnityEvent = new();
    [FoldoutGroup("Death")] public UnityEvent OnDestroyUnityEvent = new();

    
    void Awake()
    {
        deathFloorCollider.enabled = false;
        TakeDamageCaller = new PersistentFunc<DamageInfo, float>(TakeDamage);
    }

    public void SetupHealth()
    {
        healthThresholds.Sort();
        maxHealth = healthThresholds.GetHighestThreshold();

        health = new ReactiveIntercept<float>(maxHealth);
        health.Intercepts.Add(value => value < ZeroF ? ZeroF : value);
        health.Reactions.Add(CheckDie);
    }
        
    
    public float TakeDamage(DamageInfo info)
    {
        Debug.Log($"[DMG] {gameObject.name} Taking Damage: {info.dmg}, health is currently: {health.Value}");
        var newhp = health.Value -= info.dmg;
        Debug.Log("[DMG] New hp should be " + newhp + "");

        // Debug all health states compared to new hp
        healthThresholds.LogThresholds(ref healthThresholdsIndex, newhp);

        // Use while loop to ensure multiple thresholds are caught if passed at once
        while (healthThresholds.WasThresholdReached(ref healthThresholdsIndex, newhp, out var healthState))
        {
            Debug.Log($"[DMG] Threshold Triggered: {healthState} at {newhp} HP");

            if (thresholdCbs.TryGetValue(healthState, out var cb))
            {
                InvokeCb(cb, healthState);
            }
            
            if (allThresholdCb != null)
            {
                InvokeCb(allThresholdCb, healthState);
            }
        }
    
        Debug.Log("[DMG] Finished Taking Damage, final hp is " + health.Value + "");
        return newhp;
    }

    void InvokeCb(IDelegator cb, Enum healthState)
    {
        if (cb is IInvokeWithEnum invokable)
            invokable.Invoke(healthState);
        else
            Debug.LogWarning($"[DMG] Callback does not support IInvokeWithEnum for threshold {healthState}");
    }
    
    public float Heal(float amount, out Enum newState)
    {
        var newhp = health.Value += amount;
        health.Value = Mathf.Clamp(newhp, ZeroF, maxHealth);
        
        // Resync threshold index and state
        healthThresholdsIndex = healthThresholds.ResyncThresholdIndex(health.Value, out newState);
        
        Debug.Log($"[HEAL] {gameObject.name} Healed {amount}, HP: {health.Value}, State: {newState}, Next Index: {healthThresholdsIndex}");
        
        return health.Value;
    }
    
    
    void CheckDie(float v)
    {
        if (v > 0) LocationalDamageReaction();
        else Die();
    }

    void LocationalDamageReaction() 
        => damageLocationAnimHandle.PlayWeightSet(
            animator,
            DamageLocation.Body,
            initialWeight: 1, 
            endWeight: ZeroF, 
            RestartAnimation);
    
    void Die()
    {
        if (isDead) return;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        isDead = true;
        deathStatus = DeathType.Regular;
        OnDeath.Invoke(deathStatus);
        OnDeathUnityEvent.Invoke();
        deathAnimHandle.PlayWeightSet(animator, deathStatus, 1, FromBeginning);
        deathFloorCollider.enabled = true;
        foreach (var g in enableOnDeathAndUnparents) g.SetActiveThen(true).transform.parent = null;
        if (destroyOnDeath) StartCoroutine(DestroyOnDeath());
    }

    IEnumerator DestroyOnDeath()
    {
        yield return new WaitForSeconds(destroyOnDeathTime);
        OnDestroyUnityEvent.Invoke();
        Destroy(gameObject);   
    }
    
    
    
    [Button] public void TakeDamageTesting(int dmg) => health.Value -= dmg;
}