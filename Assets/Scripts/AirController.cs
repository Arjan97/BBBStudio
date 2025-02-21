using UnityEngine;

public class AirController : MonoBehaviour
{
    public int baseScore = 200;
    public float valueToAdd = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddAir()
    {
        ComboManager comboManager = FindFirstObjectByType<ComboManager>();
        BubbleBar bubbleBar = FindFirstObjectByType<BubbleBar>();

        int currentMultiplier = comboManager != null ? comboManager.multiplier : 1;
        float adjustedValueToAdd = valueToAdd * currentMultiplier; 

        if (comboManager != null)
        {
            comboManager.AddBubbleScore(baseScore);

            if (comboManager.CanIncrementMultiplier())
            {
                comboManager.IncrementMultiplier(); 
            }
        }

        if (bubbleBar != null)
        {
            bubbleBar.AddValue(adjustedValueToAdd);
        }

        // if (playerC != null)
        // {
        //     playerC.PushPlayer(7);
        // }
    }
}
