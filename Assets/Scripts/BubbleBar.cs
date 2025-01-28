using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class BubbleBar : MonoBehaviour
{
    public float maxValue = 100f;
    public float decreaseRate = 5f;
    public float currentValue;
    public Slider bubbleSlider;

    public TMP_Text scoreText;
    public TMP_Text comboText; 
    public float scoreIncreaseRate = 50f;
    public int bubbleBonusScore = 200;
    private Vector3 originalScoreTextScale;
    private Vector3 originalComboTextScale;
    private float displayedScore = 0;

    private PlayerController playerController;
    public AudioClip[] se;
    private AudioSource audio;
    private Image bubbleBarFillImage;
    public Color hitColor = Color.red;
    private Color originalColor;

    public float scoreScaleIncr = 1.1f;
    public float comboBounceScaleIncr = 1.2f; 
    public float bounceDuration = 0.1f; 

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        currentValue = maxValue;
        playerController = FindObjectOfType<PlayerController>();

        if (bubbleSlider != null)
        {
            bubbleSlider.maxValue = maxValue;
            bubbleSlider.value = currentValue;
            bubbleBarFillImage = bubbleSlider.fillRect.GetComponentInChildren<Image>();
            if (bubbleBarFillImage != null)
            {
                originalColor = bubbleBarFillImage.color;
            }
        }

        if (scoreText != null)
        {
            originalScoreTextScale = scoreText.transform.localScale;
            scoreText.text = ScoreManager.Instance.GetScore().ToString();
        }

        if (comboText != null)
        {
            originalComboTextScale = comboText.transform.localScale;
        }
    }

    private void Update()
    {
        int currentScore = ScoreManager.Instance.GetScore();

        if (Mathf.Abs(displayedScore - currentScore) > 0.1f)
        {
            displayedScore = Mathf.Lerp(displayedScore, currentScore, Time.deltaTime * 10);
            if (scoreText != null)
            {
                scoreText.text = Mathf.CeilToInt(displayedScore).ToString();
            }
        }

        if (currentValue > 0)
        {
            currentValue -= decreaseRate * Time.deltaTime;
            currentValue = Mathf.Max(currentValue, 0);
        }
        else
        {
            TriggerGameOver();
        }

        if (bubbleSlider != null)
        {
            bubbleSlider.value = currentValue;
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

        if (audio != null && se.Length > 0)
        {
            audio.clip = se[Random.Range(0, se.Length)];
            audio.Play();
        }
    }

    public void AddScore(int bonus)
    {
        ScoreManager.Instance.AddScore(bonus);

        if (scoreText != null)
        {
            scoreText.text = ScoreManager.Instance.GetScore().ToString();
            StartCoroutine(BounceScoreText(scoreText, originalScoreTextScale, scoreScaleIncr));
        }
    }

    public void TriggerComboBounce()
    {
        if (comboText != null)
        {
            StartCoroutine(BounceScoreText(comboText, originalComboTextScale, comboBounceScaleIncr));
        }
    }

    private IEnumerator BounceScoreText(TMP_Text text, Vector3 originalScale, float scaleIncr)
    {
        Vector3 targetScale = originalScale * scaleIncr;
        float elapsed = 0f;

        // Scale up
        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / bounceDuration;
            text.transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            yield return null;
        }

        elapsed = 0f;

        // Scale back down
        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / bounceDuration;
            text.transform.localScale = Vector3.Lerp(targetScale, originalScale, progress);
            yield return null;
        }

        text.transform.localScale = originalScale;
    }

    private IEnumerator DelaySceneLoad(float delay)
    {
        yield return new WaitForFixedUpdate();
        PlayerPrefs.SetInt("CurrentScore", ScoreManager.Instance.GetScore());
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("GameOver");
    }

    public void TriggerGameOver()
    {
        if (playerController != null)
        {
            playerController.TriggerDeath();
        }

        if (audio != null && se.Length > 3)
        {
            audio.clip = se[3];
            audio.Play();
        }

        StartCoroutine(DelaySceneLoad(1f));
    }

    public void FlashBubbleBarColor()
    {
        if (bubbleBarFillImage != null)
        {
            StartCoroutine(FlashColorCoroutine());
        }
    }

    private IEnumerator FlashColorCoroutine()
    {
        if (bubbleBarFillImage != null)
        {
            bubbleBarFillImage.color = hitColor;
            yield return new WaitForSeconds(0.5f);
            bubbleBarFillImage.color = originalColor;
        }
    }
}
