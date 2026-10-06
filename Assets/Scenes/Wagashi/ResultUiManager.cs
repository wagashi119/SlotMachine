using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


namespace Attacking.Ui.Result
{

    public class ResultUIManager : MonoBehaviour
    {
        [Header("UIの参照")]
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Transform bonusPointsContainer;
        [SerializeField] private Button retryButton;

        [Header("ボーナスオブジェクト")]
        [SerializeField] private GameObject bonusPointPrefab;

        [Header("アニメーション設定")]
        [SerializeField] private float delayBetweenBonusPoints = 1.0f;
        [SerializeField] private float scoreCountDuration = 2.0f;

        private string scoreBeforeText = "Score: ";
        private int baseScore;
        private int totalBonusPoints;
        private int finalScore;
        private Dictionary<string, int> bonusPoints = new Dictionary<string, int>();

        private void Awake()
        {

            // リトライボタンのイベント設定
            if (retryButton != null)
                retryButton.onClick.AddListener(RetryGame);
        }

        public void ShowResults(int score, Dictionary<string, int> bonusPoints)
        {
            // リザルトパネルを表示
            resultPanel.SetActive(true);

            baseScore = score;
            this.bonusPoints = bonusPoints;

            // 初期スコア表示
            SetScore(baseScore);

            // ボーナスポイントとランク表示のコルーチンを開始
            StartCoroutine(DisplayBonusPointsAndRank());
        }

        private IEnumerator DisplayBonusPointsAndRank()
        {
            // ボーナスポイントの表示と計算
            totalBonusPoints = 0;
            float bonusHeight = bonusPointPrefab.GetComponent<RectTransform>().sizeDelta.y;
            float currentYPosition = 0;

            // 例: 時間ボーナス
            foreach (var bonus in bonusPoints)
            {
                yield return DisplayBonusPoint(bonus.Key, bonus.Value, currentYPosition);
                totalBonusPoints += bonus.Value;
                currentYPosition -= bonusHeight; // 次のボーナスポイントの位置を調整
            }

            // 最終スコアの計算
            finalScore = baseScore + totalBonusPoints;

            // スコアのアニメーション表示
            yield return AnimateScoreCount(baseScore, finalScore);

            // リトライボタンを有効化
            retryButton.interactable = true;
        }

        private IEnumerator DisplayBonusPoint(string bonusName, int points, float yPos)
        {
            // ボーナスポイントのプレハブをインスタンス化
            GameObject bonusObj = Instantiate(bonusPointPrefab, bonusPointsContainer);

            // ボーナス名と点数を設定
            TextMeshProUGUI text = bonusObj.GetComponent<TextMeshProUGUI>();
            text.text = "- " + bonusName + ": +" + points.ToString();

            // 位置を調整
            RectTransform rectTransform = bonusObj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, yPos);

            // アニメーション効果（オプション）
            // ここにアニメーション効果を追加

            yield return new WaitForSeconds(delayBetweenBonusPoints);
        }

        private IEnumerator AnimateScoreCount(int startScore, int endScore)
        {
            float elapsedTime = 0;

            while (elapsedTime < scoreCountDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / scoreCountDuration;
                int currentScore = Mathf.FloorToInt(Mathf.Lerp(startScore, endScore, t));

                SetScore(currentScore);

                yield return null;
            }

            // 最終的な値を確実に表示
            SetScore(endScore);
        }

        private void RetryGame()
        {
            // 現在のシーンを再読み込み
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void SetScore(int score)
        {
            scoreText.text = scoreBeforeText + score.ToString();
        }
    }
}