using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieChart : MonoBehaviour
{
    public Image[] imagesPieChart;
    public float[] values;

    void Start()
    {
        SetValues(values);
    }

    public void SetValues(float[] valuesToSet)
    {
        // Calculate the total once outside the loop for efficiency
        float totalAmount = 0;
        for (int i = 0; i < valuesToSet.Length; i++)
        {
            totalAmount += valuesToSet[i];
        }

        float totalFillAmount = 0;
        for (int i = 0; i < imagesPieChart.Length && i < valuesToSet.Length; i++) // Added check to prevent index out of range
        {
            float fillAmount = valuesToSet[i] / totalAmount;
            totalFillAmount += fillAmount;
            imagesPieChart[i].fillAmount = totalFillAmount;
        }
    }
}
