using System;








public sealed class FuncPredicate : IPredicate
{
    readonly Func<bool> func;
    public bool Evaluate() => func();
    public bool Evaluate<TContext>(TContext ctx) => Evaluate();
    public FuncPredicate(Func<bool> _func) => func = _func;
    public bool Resolve(object ctx) => Evaluate();
}

public sealed class FuncCtxPredicate<TCtx> : IPredicate
{
    readonly Func<TCtx, bool> func;
    public FuncCtxPredicate(Func<TCtx, bool> _func) => func = _func;
    public bool Evaluate() => throw new InvalidOperationException("Cannot Evaluate a Predicate without a Context");
    public bool Resolve(object ctx) => func((TCtx)ctx);
}

public sealed class FuncCtxLazyPredicate<TCtxRef> : IPredicate
    where TCtxRef : class
{
    TCtxRef ctxRef;
    readonly Func<TCtxRef, bool> func;
    public FuncCtxLazyPredicate(Func<TCtxRef, bool> _func, TCtxRef _ctxRef)
    {
        func = _func;
        ctxRef = _ctxRef;
    }
    public void ReplaceCtxRef(TCtxRef newCtx) => ctxRef = newCtx;
    public bool Evaluate() => func(ctxRef);
    public bool Resolve(object ctx) => func(ctxRef = (TCtxRef)ctx);
}




