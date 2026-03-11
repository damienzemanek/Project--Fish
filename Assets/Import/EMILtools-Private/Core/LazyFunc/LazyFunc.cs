using System;
using EMILtools.Core;
using Sirenix.OdinInspector;
using UnityEngine;


// Func observing some variable 
// Func needs to know when variable changes
// Func then knows the Invoke next time its get
public class LazyFuncLite<T> : ILazyFunc<T>
    where T : struct
{
    T storedFuncEvaluation;
    readonly Func<T> func;
    T ILazyFunc<T>.InvokeLazy() => storedFuncEvaluation;

    public LazyFuncLite() { }
    
    public LazyFuncLite(PersistentDelegate onChangedReEvaluate, Func<T> func)
    {
        this.func = func;  
        storedFuncEvaluation = func != null ? func.Invoke() : default(T);
        onChangedReEvaluate?.Add(Evaluate);
    }
    
    public void Dispose(PersistentDelegate observedOnChanged) => observedOnChanged.Remove(Evaluate);
    
    public static implicit operator T(LazyFuncLite<T> lazyFuncLite) => lazyFuncLite.storedFuncEvaluation;
    

    
    void Evaluate() => storedFuncEvaluation = func.Invoke();
    
}


// Func observing some variable 
// Func needs to know when variable changes
// Func then knows the Invoke next time its get
public class LazyFunc<T> : ILazyFunc<T>
    where T : struct
{
    T storedFuncEvaluation;
    readonly Func<T> func;
    T ILazyFunc<T>.InvokeLazy() => storedFuncEvaluation;
    
    [NonSerialized] PersistentDelegate _onChangedReEvaluate;

    public LazyFunc() { }
    
    public LazyFunc(PersistentDelegate onChangedReEvaluate, Func<T> func)
    {
        this.func = func;
        if(func == null) storedFuncEvaluation = default;
        else storedFuncEvaluation = func.Invoke();
        if (onChangedReEvaluate == null) _onChangedReEvaluate = null;
        else
        {
            _onChangedReEvaluate = onChangedReEvaluate;
            _onChangedReEvaluate.Add(Evaluate);
        }
    }
    
    public void Dispose() => _onChangedReEvaluate.Remove(Evaluate);
    
    public static implicit operator T(LazyFunc<T> lazyFuncLite) => lazyFuncLite.storedFuncEvaluation;
    
    
    void Evaluate() => storedFuncEvaluation = func.Invoke();
}