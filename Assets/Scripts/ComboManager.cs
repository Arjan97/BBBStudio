using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ComboManager : MonoBehaviour
{
    public float comboDuration = 5f;
    public float timeLeft;
    public int multiplier = 1;
    private int baseMultiplier = 2; 
    private float lastMultiplierTime = 0f; 
    private const float multiplierDelay = 2f;

    public Image timerCircle;
    public TMP_Text multiplierText;
    public TMP_Text scoreText;

    public float scaleSpeed = 10f;
    public float scaleMultiplier = 1.1f;

    private Vector3 originalMultiplierScale;
    private Vector3 originalScoreScale;
    private Color originalScoreColor;

    private bool isComboActive = false;
    public Color activeColor = Color.green;

    private void Start()
    {
        timeLeft = 0;
        multiplier = baseMultiplier;

        if (multiplierText != null)
            originalMultiplierScale = multiplierText.transform.localScale;

        if (scoreText != null)
        {
            originalScoreColor = scoreText.color;
            originalScoreScale = scoreText.transform.localScale;
        }

        UpdateMultiplierDisplay();
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
            }
        }
    }

    public void AddBubbleScore(int baseScore)
    {
        int scoreToAdd = baseScore * multiplier;

        ScoreManager.Instance.AddScore(scoreToAdd);

        if (scoreText != null)
            StartCoroutine(BounceAndChangeColor(scoreText));

        if (!isComboActive)
        {
            StartCombo();
        }
    }

    public bool CanIncrementMultiplier()
    {
        return Time.time - lastMultiplierTime >= multiplierDelay;
    }

    public void IncrementMultiplier()
    {
        if (CanIncrementMultiplier())
        {
            multiplier++;
            timeLeft = comboDuration;
            UpdateMultiplierDisplay();
            lastMultiplierTime = Time.time; 

            if (multiplierText != null)
                StartCoroutine(BounceMultiplierText());
        }
    }

    private void StartCombo()
    {
        isComboActive = true;
        multiplier = baseMultiplier; 
        timeLeft = comboDuration;
        EnableComboUI();
        UpdateMultiplierDisplay();
        lastMultiplierTime = Time.time; 
    }

    private void ResetCombo()
    {
        isComboActive = false;
        multiplier = baseMultiplier;
        timeLeft = 0;

        UpdateMultiplierDisplay();
        DisableComboUI();
    }

    private void UpdateMultiplierDisplay()
    {
        if (multiplierText != null)
            multiplierText.text = "x" + multiplier;
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

    private IEnumerator BounceAndChangeColor(TMP_Text text)
    {
        text.color = activeColor;
        text.transform.localScale = originalScoreScale * scaleMultiplier;

        yield return new WaitForSeconds(0.3f);

        text.color = originalScoreColor;
        text.transform.localScale = originalScoreScale;
    }
}
