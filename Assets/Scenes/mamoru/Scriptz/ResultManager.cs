using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [SerializeField] Text scoreText;

    private void Start()
    {
        if (GameData.Instance != null)
        {
            scoreText.text = GameData.Instance.score.ToString();
        }
        else
        {
            scoreText.text = "0";
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
