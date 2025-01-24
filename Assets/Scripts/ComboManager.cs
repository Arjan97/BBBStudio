using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ComboManager : MonoBehaviour
{
    public float comboDuration = 5f;
    public float timeLeft;
    public int multiplier = 1;

    public Image timerCircle;
    public TMP_Text multiplierText;
    public TMP_Text scoreText;

    public float scaleSpeed = 10f;
    public float scaleMultiplier = 1.2f;

    private int currentScore;
    private Vector3 originalMultiplierScale;
    private Vector3 originalTimerCircleScale;
    private Vector3 originalScoreScale;
    private Color originalScoreColor;

    private bool isComboActive = false;
    public Color activeColor = Color.green;



    private void Start()
    {
        timeLeft = 0;
        multiplier = 1;
        currentScore = 0;

        if (multiplierText != null)
            originalMultiplierScale = multiplierText.transform.localScale;

        if (timerCircle != null)
            originalTimerCircleScale = timerCircle.transform.localScale;

        if (scoreText != null)
        {
            originalScoreColor = scoreText.color;
            originalScoreScale = scoreText.transform.localScale;
        }

        UpdateMultiplierDisplay();
        UpdateScoreDisplay();
        DisableComboUI();
    }

    private void Update()
    {
        if (isComboActive)
        {
            timeLeft -= Time.deltaTime;

            if (timeLeft <= 0)
            {
                ResetCombo();
            }
            else if (timerCircle != null)
            {
                timerCircle.fillAmount = timeLeft / comboDuration;
                AnimateHasteEffect();
            }
        }
    }

    public void AddScore(int baseScore)
    {
        int scoreToAdd = baseScore * multiplier;
        currentScore += scoreToAdd;
        UpdateScoreDisplay();

        if (multiplierText != null)
            StartCoroutine(BounceMultiplierText());
        if (scoreText != null)
        {
            StartCoroutine(BounceAndChangeColor());
        }
    }

    public void IncrementMultiplier()
    {
        if (!isComboActive)
        {
            isComboActive = true;
            timeLeft = comboDuration;
            EnableComboUI();
        }

        multiplier++;
        timeLeft = comboDuration;

        UpdateMultiplierDisplay();

        if (multiplierText != null)
            StartCoroutine(BounceMultiplierText());
    }

    private void ResetCombo()
    {
        isComboActive = false;
        multiplier = 1;
        timeLeft = 0;

        UpdateMultiplierDisplay();
        DisableComboUI();

        if (timerCircle != null)
            timerCircle.fillAmount = 0;
    }

    private void UpdateMultiplierDisplay()
    {
        if (multiplierText != null)
            multiplierText.text = "x" + multiplier;
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
            scoreText.text = currentScore.ToString();
    }

    private void EnableComboUI()
    {
        if (multiplierText != null)
            multiplierText.gameObject.SetActive(true);

        if (timerCircle != null)
            timerCircle.gameObject.SetActive(true);
    }

    private void DisableComboUI()
    {
        if (multiplierText != null)
            multiplierText.gameObject.SetActive(false);

        if (timerCircle != null)
            timerCircle.gameObject.SetActive(false);
    }

    private IEnumerator BounceMultiplierText()
    {
        multiplierText.transform.localScale = originalMultiplierScale * scaleMultiplier;
        yield return new WaitForSeconds(0.3f);
        multiplierText.transform.localScale = originalMultiplierScale;
    }

    private IEnumerator BounceAndChangeColor()
    {
        if (scoreText != null)
        {
            scoreText.color = activeColor;
            scoreText.transform.localScale = originalScoreScale * scaleMultiplier;

            yield return new WaitForSeconds(0.5f);

            scoreText.color = originalScoreColor;
            scoreText.transform.localScale = originalScoreScale;
        }
    }

    private void AnimateHasteEffect()
    {
        float scale = Mathf.Lerp(1f, scaleMultiplier, Mathf.PingPong(Time.time * scaleSpeed, 1f));

        if (multiplierText != null)
            multiplierText.transform.localScale = originalMultiplierScale * scale;

        if (timerCircle != null)
            timerCircle.transform.localScale = originalTimerCircleScale * scale;
    }
}
