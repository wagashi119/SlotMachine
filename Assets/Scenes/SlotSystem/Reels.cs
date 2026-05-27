using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class Reels : ScriptableObject
{
    [SerializeField] protected int reelsLengh = 20;

    public abstract Vector2Int GetLengh();
    public abstract List<Symbol>[] GetAllReels();

    protected abstract void OnEnable();
}
