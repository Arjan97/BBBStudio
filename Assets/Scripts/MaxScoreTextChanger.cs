using TMPro;
using UnityEngine;

public class MaxScoreTextChanger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerPrefs.HasKey("MaxScore"))
        {
            int maxScore = PlayerPrefs.GetInt("MaxScore");
            GetComponent<TMP_Text>().text = "Max Score: " + maxScore.ToString();
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
