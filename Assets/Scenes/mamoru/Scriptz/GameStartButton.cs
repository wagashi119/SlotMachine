using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartButton : MonoBehaviour
{
    public void ChangeMovieScene()
    {
        Debug.Log("ゲームスタート");

        if (GameData.Instance != null)
        {
            GameData.Instance.ResetScore();
        }

        SceneManager.LoadScene("movie");
    }
}