using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField]
    private int currentScore;

    public float scoreIncreaseRate = 50f;

    public delegate void ScoreUpdatedHandler(int newScore);
    public event ScoreUpdatedHandler OnScoreUpdated;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        AddScore(Mathf.CeilToInt(scoreIncreaseRate * Time.deltaTime));
    }

    public int GetScore()
    {
        return currentScore;
    }

    public void AddScore(int amount)
    {
        currentScore += amount;

        OnScoreUpdated?.Invoke(currentScore);
    }

    public void ResetScore()
    {
        currentScore = 0;

        OnScoreUpdated?.Invoke(currentScore);
    }

    public void UpdateScoreDisplay(TMP_Text scoreText, float lerpSpeed)
    {
        if (scoreText != null)
        {
            float displayedScore = Mathf.Lerp(float.Parse(scoreText.text), currentScore, lerpSpeed * Time.deltaTime);
            scoreText.text = Mathf.CeilToInt(displayedScore).ToString();
        }
    }
}
