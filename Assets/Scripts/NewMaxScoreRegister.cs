using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewMaxScoreRegister : MonoBehaviour
{
    public Image newMaxScoreIndicator;
    public TMP_Text newMaxScoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int currentMaxScore = PlayerPrefs.GetInt("MaxScore", 0);
        int currentScore = PlayerPrefs.GetInt("CurrentScore", 0);
        if (currentMaxScore < currentScore) 
        { 
            newMaxScoreIndicator.enabled = true;
            newMaxScoreText.text = "Max Score: " + currentScore.ToString(); 
            PlayerPrefs.SetInt("MaxScore", currentScore);
        }
        else
        {
            newMaxScoreIndicator.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
