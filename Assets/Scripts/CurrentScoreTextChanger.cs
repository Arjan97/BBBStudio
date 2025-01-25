using UnityEngine;
using TMPro;

public class CurrentScoreTextChanger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerPrefs.HasKey("CurrentScore"))
        {
            int maxScore = PlayerPrefs.GetInt("CurrentScore");
            GetComponent<TMP_Text>().text = "Current Score: " + maxScore.ToString();
        }
        else
        {
            GetComponent<TMP_Text>().text = "0";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
