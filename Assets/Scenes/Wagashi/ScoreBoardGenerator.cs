using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardGenerator : MonoBehaviour
{
    [SerializeField] SlotManager slotManager;
    [SerializeField] Transform contentRoot;
    [SerializeField] ScoreBoardEntry entryPrefab;
    [SerializeField, Min(1)] int columnCount = 3;
    [SerializeField] Vector2 cellSize = new Vector2(160f, 100f);
    [SerializeField] Vector2 spacing = new Vector2(12f, 12f);

    readonly List<ScoreBoardEntry> generatedEntries = new List<ScoreBoardEntry>();

    void Start()
    {
        Generate();
    }

    [ContextMenu("Generate Score Board")]
    public void Generate()
    {
        if (contentRoot == null || entryPrefab == null)
        {
            Debug.LogError("ScoreBoardGenerator: Content Root and Entry Prefab must be assigned", this);
            return;
        }

        Reels sourceReels = slotManager != null
            ? slotManager.CurrentReels
            : SlotManager.LastUsedReels;
        if (sourceReels == null)
        {
            Debug.LogError("ScoreBoardGenerator: No active Reel was found on SlotManager", this);
            return;
        }

        ClearGeneratedEntries();
        ConfigureGrid();

        var uniqueSymbols = new HashSet<Symbol>();
        foreach (List<Symbol> reelSymbols in sourceReels.GetAllReels())
        {
            if (reelSymbols == null) continue;

            foreach (Symbol symbol in reelSymbols)
            {
                if (symbol == null || !uniqueSymbols.Add(symbol)) continue;

                ScoreBoardEntry entry = Instantiate(entryPrefab, contentRoot, false);
                entry.SetData(symbol);
                generatedEntries.Add(entry);
            }
        }
    }

    void ConfigureGrid()
    {
        GridLayoutGroup grid = contentRoot.GetComponent<GridLayoutGroup>();
        if (grid == null) grid = contentRoot.gameObject.AddComponent<GridLayoutGroup>();

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = Mathf.Max(1, columnCount);
        grid.cellSize = cellSize;
        grid.spacing = spacing;
        grid.childAlignment = TextAnchor.UpperLeft;
    }

    void ClearGeneratedEntries()
    {
        foreach (ScoreBoardEntry entry in generatedEntries)
        {
            if (entry == null) continue;
            if (Application.isPlaying) Destroy(entry.gameObject);
            else DestroyImmediate(entry.gameObject);
        }

        generatedEntries.Clear();
    }
}