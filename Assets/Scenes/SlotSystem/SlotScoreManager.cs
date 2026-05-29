using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SlotScoreManager : MonoBehaviour
{
    [SerializeField] SlotRandomSelect slotSelector;
    [SerializeField] int totalScore = 0;
    [SerializeField] List<Symbol> matchedSymbols = new List<Symbol>();

    public int TotalScore => totalScore;
    public IReadOnlyList<Symbol> MatchedSymbols => matchedSymbols;

    public void OnRolled(int[] _)
    {
        EvaluateScore();
    }

    /// <summary>
    /// 選出インデックスとその前後（計3つ）の絵柄を返す。index: 0=前, 1=選出, 2=後
    /// </summary>
    public Symbol[] GetSymbolsAroundSelection(int reelIndex)
    {
        var reel = slotSelector.symbols[reelIndex];
        int center = slotSelector.GetSelectSymbolIndex()[reelIndex];
        return new[]
        {
            GetSymbolAt(reelIndex, center - 1),
            GetSymbolAt(reelIndex, center),
            GetSymbolAt(reelIndex, center + 1),
        };
    }

    Symbol GetSymbolAt(int reelIndex, int symbolIndex)
    {
        int count = slotSelector.symbols[reelIndex].Count;
        symbolIndex = ((symbolIndex % count) + count) % count;
        return slotSelector.GetSymbol(new Vector2Int(reelIndex, symbolIndex));
    }

    /// <summary>
    /// 選出位置からの縦オフセット（-1=前, 0=選出, 1=後）で横一列の絵柄を取得する。
    /// </summary>
    Symbol[] GetHorizontalLine(int verticalOffset)
    {
        int[] centers = slotSelector.GetSelectSymbolIndex();
        var line = new Symbol[centers.Length];
        for (int reel = 0; reel < centers.Length; reel++)
            line[reel] = GetSymbolAt(reel, centers[reel] + verticalOffset);
        return line;
    }

    /// <summary>
    /// 左端から連続して同じ絵柄が並ぶ数を数える（3以上で成立）。
    /// </summary>
    public bool TryGetHorizontalMatch(Symbol[] line, out Symbol matchedSymbol, out int matchCount)
    {
        matchedSymbol = null;
        matchCount = 0;
        if (line == null || line.Length == 0) return false;

        matchedSymbol = line[0];
        if (matchedSymbol == null) return false;

        matchCount = 1;
        for (int i = 1; i < line.Length; i++)
        {
            if (line[i] != matchedSymbol) break;
            matchCount++;
        }

        return matchCount >= 3;
    }

    bool IsVerticalMatch(Symbol[] around)
    {
        if (around == null || around.Length != 3) return false;
        return around[0] != null && around[0] == around[1] && around[1] == around[2];
    }

    /// <summary>
    /// 揃いを判定し totalScore に加算する。加算したスコアを返す。
    /// </summary>
    public int EvaluateScore()
    {
        matchedSymbols.Clear();
        int added = 0;

        int reelCount = slotSelector.GetSelectSymbolIndex().Length;

        // 横: 選出位置の前・選出・後の3ラインそれぞれを判定
        foreach (int verticalOffset in new[] { -1, 0, 1 })
        {
            var line = GetHorizontalLine(verticalOffset);
            if (!TryGetHorizontalMatch(line, out Symbol symbol, out _)) continue;

            matchedSymbols.Add(symbol);
            added += symbol.score;
        }

        // 縦: 各リールで選出とその前後が揃っているか
        for (int reel = 0; reel < reelCount; reel++)
        {
            var around = GetSymbolsAroundSelection(reel);
            if (!IsVerticalMatch(around)) continue;

            Symbol symbol = around[1];
            matchedSymbols.Add(symbol);
            added += symbol.score;
        }

        totalScore += added;
        return added;
    }

    /// <summary>
    /// 直近の判定で揃った Symbol をコンソールに出力する。
    /// </summary>
    [ContextMenu("OutPut_Console")]
    public void PrintMatchedSymbols()
    {
        if (matchedSymbols.Count == 0)
        {
            Debug.Log("[SlotScore] 揃った絵柄はありません。");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("[SlotScore] 揃った絵柄:");
        for (int i = 0; i < matchedSymbols.Count; i++)
        {
            Symbol s = matchedSymbols[i];
            sb.AppendLine($"  {i + 1}. {s.name} (score: {s.score})");
        }

        Debug.Log(sb.ToString());
    }
}
