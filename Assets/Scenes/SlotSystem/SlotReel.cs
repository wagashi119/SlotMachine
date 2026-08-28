using System.Collections.Generic;
using UnityEngine;

public class SlotReel : MonoBehaviour
{
    [SerializeField] Reels reelsData;
    Symbol[,] reels = new Symbol[0,0];

    public Vector2Int slotLengh => reelsData.GetLengh();

    [ContextMenu("Reload")]
    private void Awake()
    {
        Vector2Int slotLengh = this.slotLengh;
        reels = new Symbol[slotLengh.x,slotLengh.y];
    }

    private Vector2Int IndexCorrection(Vector2Int index)
    {
        if (slotLengh.x <= index.x)
        {
            index.x = slotLengh.x - 1;
        }
        index.y = index.y % slotLengh.y;

        return index;
    }

    public Symbol GetSymbol(Vector2Int index)
    {
        
        return null;
    }

    public Sprite GetSymbolImage(Vector2Int index)
    {
        return null;
    }

    public int GetSymbolScore(Vector2Int index)
    {
        return 1;
    }

    
}
