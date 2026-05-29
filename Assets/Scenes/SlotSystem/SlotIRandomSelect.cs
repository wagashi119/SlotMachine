using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SlotRandomSelect : MonoBehaviour
{
    [SerializeField] Reels reels;
    [SerializeField] int[] selectSymbolIndexs = new int[0];
    public UnityEvent<int[]> onRolled = new UnityEvent<int[]>();

    public int slotsCount => reels.GetLengh().x;
    public List<Symbol>[] symbols = new List<Symbol>[0];

    private void Awake()
    {
        symbols = new List<Symbol>[reels.GetLengh().x];
        symbols = reels.GetAllReels();
    }


    [ContextMenu("rollSlot")]
    public void Roll()
    {
        List<Symbol>[] reels = this.reels.GetAllReels();
        selectSymbolIndexs = new int[reels.Length];
        
        for (int i=0; i<reels.Length; i++)
        {
            // 配???からランダ??にセレク??

            int selectIndex = Random.Range(0, reels[i].Count);

            selectSymbolIndexs[i] = selectIndex;
        }

        onRolled.Invoke(selectSymbolIndexs);
    }

    public int[] GetSelectSymbolIndex() => selectSymbolIndexs;

    public Symbol GetSymbol(Vector2Int index)
    {
        Vector2Int lengh = reels.GetLengh();
        if (index.x >= lengh.x || index.y >= lengh.y || index.x < 0 || index.y < 0) return null;

        Symbol selectSymbol = symbols[index.x][index.y];
        if (selectSymbol)
        {
            return selectSymbol;
        }

        Debug.LogError($"番号:{lengh}が{selectSymbol}です");
        return null;
    }
}
