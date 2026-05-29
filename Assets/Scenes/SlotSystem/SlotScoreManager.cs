using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SlotScoreManager : MonoBehaviour
{
    [SerializeField] SlotRandomSelect symbolSelector;
    [SerializeField] int totalScore = 0;
    [SerializeField] List<Symbol> matchedSymbols = new List<Symbol>();

    [Header("判定範囲")]
    [Tooltip("選出位置を中心に、上下へ何段ずつ見るか。3 なら前・選出・後の3段。")]
    [SerializeField] int verticalLineCount = 3;
    [Tooltip("選出位置を中心に、評価する横ライン（段）の本数。3 なら上・選出・下の3本。")]
    [SerializeField] int horizontalLineCount = 3;
    [Tooltip("揃い成立に必要な最少個数（横は左から連続、縦は列内すべて一致）。")]
    [SerializeField] int minMatchCount = 3;

    public int TotalScore => totalScore;
    public IReadOnlyList<Symbol> MatchedSymbols => matchedSymbols;
    public int VerticalLineCount => verticalLineCount;
    public int HorizontalLineCount => horizontalLineCount;
    public int MinMatchCount => minMatchCount;

    void OnValidate()
    {
        minMatchCount = Mathf.Max(2, minMatchCount);
        horizontalLineCount = Mathf.Max(1, horizontalLineCount);
        verticalLineCount = Mathf.Max(minMatchCount, verticalLineCount);
    }

    public void OnRolled(int[] _)
    {
        EvaluateScore();
    }

    /// <summary>
    /// 横ライン評価用の縦オフセット（例: 3 本なら -1, 0, 1）。
    /// </summary>
    public int[] GetHorizontalRowOffsets()
    {
        return BuildCenteredOffsets(horizontalLineCount);
    }

    /// <summary>
    /// 1 リール分の縦列（選出位置を中心に verticalLineCount 個）。
    /// </summary>
    public Symbol[] GetVerticalColumn(int reelIndex)
    {
        int center = symbolSelector.GetSelectSymbolIndex()[reelIndex];
        int[] offsets = BuildCenteredOffsets(verticalLineCount);
        var column = new Symbol[offsets.Length];
        for (int i = 0; i < offsets.Length; i++)
            column[i] = symbolSelector.GetSymbol(reelIndex, center+offsets[i]);
        return column;
    }

    /// <summary>
    /// 選出位置からの縦オフセットで、全リール横一列の絵柄を取得する。
    /// </summary>
    public Symbol[] GetHorizontalRow(int verticalOffset)
    {
        int[] centers = new int[symbolSelector.slotsCount];
        centers = symbolSelector.GetSelectSymbolIndex();
        var row = new Symbol[centers.Length];
        for (int reel = 0; reel < centers.Length; reel++)
            row[reel] = symbolSelector.GetSymbol(reel, centers[reel] + verticalOffset);
        return row;
    }

    /// <summary>
    /// 縦列が揃っているか（列内すべて同じ Symbol かつ個数が minMatchCount 以上）。
    /// </summary>
    public bool TryGetVerticalMatch(int reelIndex, out Symbol matchedSymbol, out int matchCount)
    {
        return TryGetVerticalMatch(GetVerticalColumn(reelIndex), out matchedSymbol, out matchCount);
    }

    public bool TryGetVerticalMatch(Symbol[] column, out Symbol matchedSymbol, out int matchCount)
    {
        matchedSymbol = null;
        matchCount = 0;
        if (column == null || column.Length < minMatchCount) return false;

        matchedSymbol = column[0];
        if (matchedSymbol == null) return false;

        matchCount = 1;
        for (int i = 1; i < column.Length; i++)
        {
            if (column[i] != matchedSymbol) return false;
            matchCount++;
        }

        return matchCount >= minMatchCount;
    }

    /// <summary>
    /// 左端から連続して同じ絵柄が並ぶか（minMatchCount 以上で成立）。
    /// </summary>
    public bool TryGetHorizontalMatch(Symbol[] row, out Symbol matchedSymbol, out int matchCount)
    {
        matchedSymbol = null;
        matchCount = 0;
        if (row == null || row.Length < minMatchCount) return false;

        matchedSymbol = row[0];
        if (matchedSymbol == null) return false;

        matchCount = 1;
        for (int i = 1; i < row.Length; i++)
        {
            if (row[i] != matchedSymbol) break;
            matchCount++;
        }

        return matchCount >= minMatchCount;
    }

    /// <summary>
    /// 縦列の揃いだけを判定し、スコアを加算する。加算分を返す。
    /// </summary>
    public int AddScoreFromVerticalMatches()
    {
        int added = 0;
        int reelCount = symbolSelector.GetSelectSymbolIndex().Length;

        for (int reel = 0; reel < reelCount; reel++)
        {
            if (!TryGetVerticalMatch(reel, out Symbol symbol, out _)) continue;

            matchedSymbols.Add(symbol);
            added += symbol.score;
        }

        return added;
    }

    /// <summary>
    /// 横列の揃いだけを判定し、スコアを加算する。加算分を返す。
    /// </summary>
    public int AddScoreFromHorizontalMatches()
    {
        int added = 0;
        foreach (int offset in GetHorizontalRowOffsets())
        {
            var row = GetHorizontalRow(offset);
            if (!TryGetHorizontalMatch(row, out Symbol symbol, out _)) continue;

            matchedSymbols.Add(symbol);
            added += symbol.score;
        }

        return added;
    }

    /// <summary>
    /// 揃いを判定し totalScore に加算する。加算したスコアを返す。
    /// </summary>
    public int EvaluateScore()
    {
        matchedSymbols.Clear();
        int added = AddScoreFromHorizontalMatches();
        added += AddScoreFromVerticalMatches();
        totalScore += added;
        return added;
    }

    static int[] BuildCenteredOffsets(int lineCount)
    {
        int start = -(lineCount / 2);
        var offsets = new int[lineCount];
        for (int i = 0; i < lineCount; i++)
            offsets[i] = start + i;
        return offsets;
    }

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
