using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class Reels : ScriptableObject
{
    [SerializeField] protected int reelsLengh = 20;
    public abstract Vector2 Get();
    public abstract List<Symbol>[] GetReels();

    protected abstract void OnEnable();
}
