using System;
using EMILtools.Systems;

public interface IPredicate : IResolvable
{
    bool Evaluate();
}

public sealed class NotPredicate : IPredicate
{
    private readonly IPredicate _inner;

    public NotPredicate(IPredicate inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public bool Evaluate() => !_inner.Evaluate();
    public bool consumed => false;
    public void ResetWait()
    {
        // No op
    }

    public bool Resolve(object ctx) => !_inner.Resolve(ctx);
}

