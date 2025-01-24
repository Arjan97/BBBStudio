using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BubbleBar : MonoBehaviour
{
    public float maxValue = 100f;
    public float decreaseRate = 5f;
    public float currentValue;
    public Slider bubbleSlider;

    public TMP_Text scoreText;
    public float scoreIncreaseRate = 50f;
    public int bubbleBonusScore = 200;
    private int currentScore;
    private Vector3 originalScoreTextScale;
    private float displayedScore = 0;

    public TMP_Text comboPrefab;

    private void Start()
    {
        currentValue = maxValue;
        if (bubbleSlider != null)
        {
            bubbleSlider.maxValue = maxValue;
            bubbleSlider.value = currentValue;
        }
        if (scoreText != null)
        {
            originalScoreTextScale = scoreText.transform.localScale;
            scoreText.text = currentScore.ToString();
        }
    }

    private void Update()
    {
        if (Mathf.Abs(displayedScore - currentScore) > 0.1f)
        {
            displayedScore = Mathf.Lerp(displayedScore, currentScore, Time.deltaTime * 10);
            scoreText.text = Mathf.CeilToInt(displayedScore).ToString();
        }

        if (currentValue > 0)
        {
            currentValue -= decreaseRate * Time.deltaTime;
            currentValue = Mathf.Max(currentValue, 0);
        }
        if (bubbleSlider != null)
        {
            bubbleSlider.value = currentValue;
        }

        currentScore += Mathf.CeilToInt(scoreIncreaseRate * Time.deltaTime);
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    public void AddValue(float amount)
    {
        currentValue += amount;
        currentValue = Mathf.Min(currentValue, maxValue);
        if (bubbleSlider != null)
        {
            bubbleSlider.value = currentValue;
        }
    }

    public void AddScore(int bonus)
    {
        currentScore += bonus;
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
            StartCoroutine(BounceScoreText());
        }
    }

    private IEnumerator BounceScoreText()
    {
        scoreText.transform.localScale = originalScoreTextScale * 1.5f;
        yield return new WaitForSeconds(0.1f);
        scoreText.transform.localScale = originalScoreTextScale;
    }

    private IEnumerator FlashScoreText()
    {
        Color originalColor = scoreText.color;
        scoreText.color = Color.yellow;
        yield return new WaitForSeconds(0.2f);
        scoreText.color = originalColor;
    }

    private void ShowComboFeedback()
    {
        TMP_Text comboText = Instantiate(comboPrefab, scoreText.transform.position, Quaternion.identity, scoreText.transform);
        comboText.text = "Combo x2!";
        Destroy(comboText.gameObject, 1f);
    }
}
