using System;
using UnityEngine;

[Serializable]
public abstract class InputMap : IInputMap
{
    [SerializeField] public string ownerName;
    public InputMap() { }
    public InputMap(string ownerName) => this.ownerName = ownerName;
}