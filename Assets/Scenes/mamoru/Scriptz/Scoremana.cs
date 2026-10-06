using UnityEngine;
using System.Collections.Generic;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    private int score = 0;
    private IReadOnlyList<MatchState> slotScores = new List<MatchState>();

    public void SetScore(int score, List<MatchState> slotScores)
    {
        this.score = score;
        this.slotScores = slotScores;
    }

    public (int, IReadOnlyList<MatchState>) GetScore()
    {
        return (score, slotScores);
    }

    private void Awake()
    {
        // ���łɑ��݂��Ă�����폜
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ResetScore()
    {
        score = 0;
        slotScores = new List<MatchState>().AsReadOnly();
    }
}