using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class ResultManager : MonoBehaviour
{
    [Header("Score Display")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] string scoreBeforeText = "";
    [SerializeField] string scoreAfterText = "pts";
    [SerializeField] float scoreCountDuration = 1.0f;

    [Header("Matcheds Display")]
    [SerializeField] Transform matchedSymbolsContainer;
    [SerializeField] GameObject matchedSymbolPrefab;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip matchSound;
    [SerializeField] float matchAnimationDelay = 0.5f;
    Dictionary<Sprite, int> matchedSymbols = new Dictionary<Sprite, int>();

    [Header("NextButtons")]
    [SerializeField] bool canRestart = false;

    void Awake()
    {
        if (scoreText)
        {
            scoreText.text = $"{scoreBeforeText}???{scoreBeforeText}";
        }
    }

    private void Start()
    {
        StartCoroutine(DisplayAnimation());
    }

     Dictionary<Sprite, int> ConvertSlotScores(IEnumerable<MatchState> slotScores)
    {
        return slotScores.GroupBy(matchState => matchState.image).ToDictionary(g => g.Key, g => g.Sum(ms => ms.gatherCount));
    }

    private IEnumerator DisplayAnimation()
    {
        var (score, slotScores) = GameData.Instance.GetScore();
        matchedSymbols = ConvertSlotScores(slotScores);

        foreach (var matchedSymbol in matchedSymbols)
        {
            GameObject symbolObj = Instantiate(matchedSymbolPrefab, matchedSymbolsContainer);
            symbolObj.GetComponent<UnityEngine.UI.Image>().sprite = matchedSymbol.Key;
            symbolObj.GetComponentInChildren<TextMeshProUGUI>().text = matchedSymbol.Value.ToString();
            Debug.Log($"Displaying matched symbol: {matchedSymbol.Key.name} with count: {matchedSymbol.Value}");


            if (audioSource != null && matchSound != null)
            {
                audioSource.PlayOneShot(matchSound);
            }

            yield return new WaitForSeconds(matchAnimationDelay);
        }

        yield return StartCoroutine(ResultScoreAnimation(score));
    }

    private IEnumerator ResultScoreAnimation(int score)
    {
        float elapsedTime = 0;

        while (elapsedTime < scoreCountDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / scoreCountDuration;
            int currentScore = Mathf.FloorToInt(Mathf.Lerp(0, score, t));

            if (scoreText != null)
            {
                scoreText.text = $"{scoreBeforeText}{currentScore}{scoreAfterText}";
            }
            yield return null;
        }

        canRestart = true;
        Debug.Log($"Score: {score}");
    }

    public void Restart()
    {
        if (!canRestart)
        {
            Debug.LogWarning("ResultManager: Cannot restart yet, animation not finished.");
            return;
        }

        if (GameData.Instance != null)
        {
            GameData.Instance.ResetScore();
        }

        SceneManager.LoadScene("SampleScene");
    }
    public void BackToTitle()
    {
        if (!canRestart)
        {
            Debug.LogWarning("ResultManager: Cannot restart yet, animation not finished.");
            return;
        }

        if (GameData.Instance != null)
        {
            GameData.Instance.ResetScore();
        }

        SceneManager.LoadScene("Start");
    }
}
