using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public int score = 0;

    private void Awake()
    {
        // Ç∑Ç≈Ç…ë∂ç›ÇµÇƒÇ¢ÇΩÇÁçÌèú
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
    }
}