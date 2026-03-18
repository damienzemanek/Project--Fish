using System;

public sealed class FuncPredicate : IPredicate
{
    readonly Func<bool> func;
    public bool Evaluate() => func();
    FuncPredicate(Func<bool> _func) => func = _func;
}