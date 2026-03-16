using System;

[Serializable]
public struct ObjectEnumNamed<TEnum, TObject>
    where TEnum : Enum
{
    public TEnum label;
    public TObject obj;
}