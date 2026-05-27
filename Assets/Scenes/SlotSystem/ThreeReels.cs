using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ThreeReels", menuName = "Reels/ThreeReels")]
class ThreeReels : Reels
{
    [SerializeField] List<Symbol> reels1 = new List<Symbol>(20);
    [SerializeField] List<Symbol> reels2 = new List<Symbol>(20);
    [SerializeField] List<Symbol> reels3 = new List<Symbol>(20);

    public override List<Symbol>[] GetAllReels()
    {
        return new List<Symbol>[3]
        {
            reels1, reels2, reels3
        };
    }

    public override Vector2Int GetLengh()
    {
        return new Vector2Int(3, reelsLengh);
    }


    protected override void OnEnable()
    {
        // リストの長さを調整したりしなかったりする
        //　後でやる
    }
}