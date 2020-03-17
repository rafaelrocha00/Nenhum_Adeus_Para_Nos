using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personality
{
    float[] percentages = new float[5];

    public void SetPercentage(int idx, float percentage)
    {
        if (idx >= 0 && idx < 5) percentages[idx] = percentage;
    }

    public float CalculatePercentage(DialogueBattle.ApproachType apType)
    {
        return percentages[(int)apType];
    }
}
