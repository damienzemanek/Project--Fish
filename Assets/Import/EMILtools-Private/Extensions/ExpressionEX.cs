using System;
using System.Linq.Expressions;

public static class ExpressionEX 
{
    public static bool ExprEqual<T>(Expression<Func<T, T>> a, Expression<Func<T, T>> b)
    {
        if (a == null || b == null) return a == b;
        return a.ToString() == b.ToString();
    }
}
