using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public int startValue = 360; 
    private int currentValue;    

    public TextMeshProUGUI timerText; 
    public Animator playerAnimator;  
    public GameObject gameOverScreen; 
    public KeyCode toggleKey = KeyCode.P; // Tecla para alternar a visibilidade

    void Start()
    {
        currentValue = startValue; 
        UpdateTimerUI(); 
        InvokeRepeating(nameof(DecrementTimer), 1f, 1f); 
    }

    void DecrementTimer()
    {
        if (currentValue > 0)
        {
            currentValue--; 
            UpdateTimerUI(); 
        }
        else
        {
            CancelInvoke(nameof(DecrementTimer)); 
            TriggerGameOver();
        }
    }

    void UpdateTimerUI()
    {
        timerText.text = currentValue.ToString(); 
    }

    void TriggerGameOver()
    {
        playerAnimator.SetTrigger("Dead");

        Invoke(nameof(ShowGameOverScreen), 2f);
    }

    void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true); 
    }
}


    