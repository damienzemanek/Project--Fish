using System.Collections;
using System.Collections.Generic;
using EMILtools.Core;
using EMILtools.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using static EMILtools.Extensions.NumEX;
using static IDamageable;

public class LivingEntity : Entity,
    IDamageable
{
    const float RestartAnimation = ZeroF;
    const float FromBeginning = ZeroF;
    
    [FoldoutGroup("ReadOnly")] [ShowInInspector, ReadOnly] public bool isDead = false;
    [FoldoutGroup("ReadOnly")] [ShowInInspector] ReactiveIntercept<float> health;
    [FoldoutGroup("ReadOnly")] [ShowInInspector, ReadOnly] public DeathType deathStatus;
    
    public float maxHealth;
    [SerializeField] int deathLayer = 3;
    [SerializeField] int hitLayer = 2; 
    public bool destroyOnDeath = false;
    public List<Behaviour> behaviours = new();
    public List<Collider2D> colliders = new();
    public Collider2D deathFloorCollider;
    public List<GameObject> enableOnDeathAndUnparents = new();

    public Animator animator;
    public AnimHandle<DeathType, NoBlends> deathAnimHandle;
    public AnimHandle<DamageLocation, NoBlends> damageLocationAnimHandle;
    [HideInInspector] public PersistentAction<DeathType> OnDeath = new();

    public UnityEvent OnDie = new UnityEvent();
    
    void Awake()
    {
        deathFloorCollider.enabled = false;
        health = new ReactiveIntercept<float>(maxHealth);
        health.Intercepts.Add(value => value < ZeroF ? ZeroF : value);
        health.Reactions.Add(CheckDie);
    }
    
    public void TakeDamage(DamageInfo info)
    {
        Debug.Log("Taking Damage");
        health.Value -= info.dmg;
    }
    
    [Button]
    public void TakeDamage(int dmg) => health.Value -= dmg;

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
        isDead = true;
        deathStatus = DeathType.Regular;
        OnDeath.Invoke(deathStatus);
        deathAnimHandle.PlayWeightSet(animator, deathStatus, 1, deathLayer, FromBeginning);
        foreach (var b in behaviours) b.enabled = false;
        foreach (var c in colliders) c.enabled = false;
        deathFloorCollider.enabled = true;
        foreach (var g in enableOnDeathAndUnparents) g.SetActiveThen(true).transform.parent = null;
        OnDie?.Invoke();
        if (destroyOnDeath) StartCoroutine(DestroyOnDeath());
    }

    IEnumerator DestroyOnDeath()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);   
    }

}