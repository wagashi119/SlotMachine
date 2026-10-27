using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;
using WagashiTool.Types;
using System.Collections;

public class ResultManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] string scoreBeforeText = "";
    [SerializeField] string scoreAfterText = "px";
    [SerializeField] float scoreCountDuration = 1.0f;

    [Header("Matched Symbols")]
    [SerializeField] Transform matchedSymbolsContainer;
    [SerializeField] GameObject matchedSymbolPrefab;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip matchSound;
    [SerializeField] float matchAnimationDelay = 0.5f;
    [SerializeField] List<SerializeTuple<Sprite, int>> matchedSymbols = new List<SerializeTuple<Sprite, int>>();

    private void Start()
    {
        StartCoroutine(DisplayAnimation());}

     List<SerializeTuple<Sprite, int>> ConvertSlotScores(IEnumerable<MatchState> slotScores)
    {
        return slotScores.Select(matchState => new SerializeTuple<Sprite, int>(matchState.image, matchState.gatherCount)).ToList();
    }

    private IEnumerator DisplayAnimation()
    {
        var (score, slotScores) = GameData.Instance.GetScore();
        foreach (var matchedSymbol in matchedSymbols)
        {
            GameObject symbolObj = Instantiate(matchedSymbolPrefab, matchedSymbolsContainer);
            symbolObj.GetComponent<UnityEngine.UI.Image>().sprite = matchedSymbol.item1;
            symbolObj.GetComponentInChildren<TextMeshProUGUI>().text = matchedSymbol.item2.ToString();
            Debug.Log($"Displaying matched symbol: {matchedSymbol.item1.name} with count: {matchedSymbol.item2}");


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

        Debug.Log($"Score: {score}");
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
