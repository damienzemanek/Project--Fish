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
    IHealable<BasicHealthThresholds>,
    ILivingEntityControlled
{
    public enum BasicHealthThresholds { Alive, Dying, Dead }

    const float RestartAnimation = ZeroF;
    const float FromBeginning = ZeroF;
    float maxHealth;


    [FoldoutGroup("ReadOnly")] [ShowInInspector] ReactiveIntercept<float> health;
    [FoldoutGroup("ReadOnly")] [ShowInInspector, ReadOnly] public bool isDead = false;
    [FoldoutGroup("ReadOnly")] [ShowInInspector, ReadOnly] public DeathType deathStatus;
    
    [FoldoutGroup("Settings")] public Threshold<BasicHealthThresholds, PersistentAction<BasicHealthThresholds>> healthThresholds = new();
    [FoldoutGroup("Settings")] [SerializeField] int deathLayer = 3;
    [FoldoutGroup("Settings")] [SerializeField] int hitLayer = 2; 
    
    [FoldoutGroup("Death")] public bool destroyOnDeath = false;
    [FoldoutGroup("Death")] [ShowIf("destroyOnDeath")] public float destroyOnDeathTime = 2f;
    [FoldoutGroup("Death")] [Required] public Rigidbody2D rb;
    [FoldoutGroup("Death")] [Required] public Collider2D deathFloorCollider;
    [FoldoutGroup("Death")] public List<GameObject> enableOnDeathAndUnparents = new();
    [FoldoutGroup("Death")] public UnityEvent OnDeathUnityEvent = new();

    
    [FoldoutGroup("Animation")] [Required] public Animator animator;
    [FoldoutGroup("Animation")] public AnimHandle<DeathType, NoBlends> deathAnimHandle;
    [FoldoutGroup("Animation")] public AnimHandle<DamageLocation, NoBlends> damageLocationAnimHandle;

    public PersistentFunc<DamageInfo, float> TakeDamageCaller { get; private set; }
    public PersistentAction<DeathType> OnDeath { get; set; } = new();
    
    void Awake()
    {
        healthThresholds.Reset();
        maxHealth = healthThresholds.GetHighestThreshold();

        health = new ReactiveIntercept<float>(maxHealth);
        health.Intercepts.Add(value => value < ZeroF ? ZeroF : value);
        health.Reactions.Add(CheckDie);

        healthThresholds.SyncIndexToValue(health.Value);

        deathFloorCollider.enabled = false;
        TakeDamageCaller = new PersistentFunc<DamageInfo, float>(TakeDamage);
    }
    
    public float TakeDamage(DamageInfo info)
    {
        Debug.Log($"{gameObject.name} Taking Damage");
        var newhp = health.Value -= info.dmg;
        if(healthThresholds.WasThresholdReached(newhp, out var healthState, out var cb)) 
            cb.Invoke(healthState);
        return newhp;
    }
    
    public float Heal(float amount, out BasicHealthThresholds newState)
    {
        var newhp = health.Value += amount;
        var clamped = Mathf.Clamp(newhp, ZeroF, maxHealth);
        // Rewind threshold progression to match healed HP
        healthThresholds.SyncIndexToValue(clamped);
        healthThresholds.GetNearestLastThreshold(newhp, out newState, out _);
        return newhp;
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
            hitLayer, 
            RestartAnimation);
    
    void Die()
    {
        if (isDead) return;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        isDead = true;
        deathStatus = DeathType.Regular;
        OnDeath.Invoke(deathStatus);
        deathAnimHandle.PlayWeightSet(animator, deathStatus, 1, deathLayer, FromBeginning);
        deathFloorCollider.enabled = true;
        foreach (var g in enableOnDeathAndUnparents) g.SetActiveThen(true).transform.parent = null;
        if (destroyOnDeath) StartCoroutine(DestroyOnDeath());
    }

    IEnumerator DestroyOnDeath()
    {
        yield return new WaitForSeconds(destroyOnDeathTime);
        Destroy(gameObject);   
    }
    
    
    
    [Button] public void TakeDamageTesting(int dmg) => health.Value -= dmg;
}