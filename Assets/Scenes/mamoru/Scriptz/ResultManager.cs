using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI matchedSymbolsText;
    [SerializeField] List<MatchState> matchedSymbols = new List<MatchState>();

    [ContextMenu("Set Score and Matched Symbols")]
    private void Awake()
    {
        if (GameData.Instance != null)
        {
            var (score, slotScores) = GameData.Instance.GetScore();
            matchedSymbols = slotScores.ToList();
            scoreText.text = score.ToString();
            if (matchedSymbolsText != null)
            {
                Debug.Log($"Score: {score}, Matched Symbols Count: {slotScores.Count}");
                matchedSymbolsText.text = "";
                matchedSymbolsText.text = string.Join("\n", matchedSymbols.Select(matchState => $"Symbol: {matchState.score}, Count: {matchState.gatherCount}"));
            }
        }
    }

    public void Restart()
    {
        if (GameData.Instance != null)
        {
            GameData.Instance.ResetScore();
        }

        SceneManager.LoadScene("SampleScene");
    }
    public void BackToTitle()
    {
        if (GameData.Instance != null)
        {
            GameData.Instance.ResetScore();
        }

        SceneManager.LoadScene("Start");
    }
}
