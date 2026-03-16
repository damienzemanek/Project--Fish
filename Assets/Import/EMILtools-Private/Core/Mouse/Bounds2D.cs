using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class Bounds2D<TEnum>
    where TEnum : Enum
{
    public ObjectEnumNamed<TEnum, PolygonCollider2D>[] colliders;
    
    public TEnum GetBound(Vector2 mousePos)
    {
        Vector2 worldMouse = Camera.main.ScreenToWorldPoint(mousePos);
        foreach (var bound in colliders)
        {
            bool hit = bound.obj.OverlapPoint(worldMouse);
            if (hit) return bound.label;
        }
        return default;
    }
}