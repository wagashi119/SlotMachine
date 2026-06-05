using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SlotRandomSelect : MonoBehaviour
{
    Reels reels;
    [SerializeField] int[] selectSymbolIndexs = new int[0];
    public UnityEvent<int[]> onRolled = new UnityEvent<int[]>();

    public int slotsCount => reels.GetLengh().x;
    public List<Symbol>[] symbols = new List<Symbol>[0];

    public void ChangeReels(Reels reels) {
        this.reels = reels;
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
            // �z???���烉���_??�ɃZ���N??

            int selectIndex = Random.Range(0, reels[i].Count);

            selectSymbolIndexs[i] = selectIndex;
        }

        onRolled.Invoke(selectSymbolIndexs);
    }

    public int[] GetSelectSymbolIndex() {
        //Debug.Log($"lengh:{selectSymbolIndexs.Length}, {selectSymbolIndexs == null}");

        return selectSymbolIndexs;
    }

    public Symbol GetSymbol(Vector2Int index)
    {
        Vector2Int lengh = reels.GetLengh();
        if (index.x >= lengh.x || index.x < 0 || lengh.y == 0) return null;
        index.y =((index.y % lengh.y) + lengh.y) % lengh.y;

        Symbol selectSymbol = symbols[index.x][index.y];
        if (selectSymbol)
        {
            return selectSymbol;
        }

        Debug.LogError($"�ԍ�:{lengh}��{selectSymbol}�ł�");
        return null;
    }
    public Symbol GetSymbol(int reelIndex, int symbolIndex)
    {
        return GetSymbol(new Vector2Int(reelIndex, symbolIndex));
    }
}
