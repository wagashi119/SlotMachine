using System.Collections.Generic;
using UnityEngine;

public class SlotRandomSelect : MonoBehaviour
{
    [SerializeField] Reels reels;
    [SerializeField] List<Symbol> selectSymbols = new List<Symbol>(3);
    [SerializeField] List<SpriteRenderer> slotRenderer = new List<SpriteRenderer>();

    [ContextMenu("rollSlot")]
    public void Roll()
    {
        List<Symbol>[] reels = this.reels.GetReels();
        selectSymbols.Clear();
        
        for (int i=0; i<reels.Length; i++)
        {
            // 配列からランダムにセレクト

            int selectIndex = Random.Range(0, reels[i].Count);
            Symbol selectSymbol = reels[i][selectIndex];

            selectSymbols.Add(selectSymbol);

            if (slotRenderer.Count > i && slotRenderer[i])
            {
                slotRenderer[i].sprite = selectSymbol.image;
            }
        }
    }
}
