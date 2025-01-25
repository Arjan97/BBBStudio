using UnityEngine;

public class SpriteCleaner : MonoBehaviour
{
    public float cleanness = 0.0f; // from 0.0 to 1.0
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
        Color pollutedClr = pollutetSprite.color;
        pollutedClr.a = cleanness;
        pollutetSprite.color = pollutedClr;

        Color cleanClr = cleanSprite.color;
        cleanClr.a = 1.0f - cleanness;
        cleanSprite.color = cleanClr;
    }
}
