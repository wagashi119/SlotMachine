using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreBoardEntry : MonoBehaviour
{
    [SerializeField] Image symbolImage;
    [SerializeField] TextMeshProUGUI symbolNameText;
    [SerializeField] TextMeshProUGUI scoreText;

    public void SetData(Symbol symbol)
    {
        if (symbol == null)
        {
            Debug.LogWarning("ScoreBoardEntry: Symbol is null", this);
            return;
        }

        if (symbolImage == null) symbolImage = GetComponentInChildren<Image>(true);
        if (symbolNameText == null || scoreText == null)
        {
            TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>(true);
            if (symbolNameText == null && texts.Length > 0) symbolNameText = texts[0];
            if (scoreText == null && texts.Length > 1) scoreText = texts[1];
        }

        if (symbolImage != null) symbolImage.sprite = symbol.image;
        if (symbolNameText != null) symbolNameText.text = symbol.name;
        if (scoreText != null) scoreText.text = symbol.score.ToString();
    }
}