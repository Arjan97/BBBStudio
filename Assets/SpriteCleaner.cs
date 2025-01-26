using UnityEngine;

public class SpriteCleaner : MonoBehaviour
{
    float cleanliness = 0.0f; // from 0.0 to 1.0
    SpriteRenderer pollutetSprite;
    SpriteRenderer cleanSprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pollutetSprite = GetComponent<SpriteRenderer>();
        cleanSprite = transform.GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("ScoreManager.Instance.GetScore():" + ScoreManager.Instance.GetScore() + "; ScoreManager.Instance.cleanScore:" + ScoreManager.Instance.cleanScore);
        cleanliness = ScoreManager.Instance.GetScore() / ScoreManager.Instance.cleanScore;
        if (cleanliness > 1.0f) {
            cleanliness = 1.0f;
        }

        Color pollutedClr = pollutetSprite.color;
        pollutedClr.a = cleanliness;
        pollutetSprite.color = pollutedClr;

        Color cleanClr = cleanSprite.color;
        cleanClr.a = 1.0f - cleanliness;
        cleanSprite.color = cleanClr;
    }
}
