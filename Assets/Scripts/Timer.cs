using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public int startValue = 360; // Tempo inicial em segundos (6 minutos)
    private int currentValue;    // Valor atual do timer

    public TextMeshProUGUI[] timerTexts; // Array de TextMeshPro para exibir o tempo em múltiplos lugares
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
        // Converte o tempo para o formato minutos:segundos
        int minutes = currentValue / 60;
        int seconds = currentValue % 60;
        string formattedTime = $"{minutes:D2}:{seconds:D2}"; // Formata como "MM:SS"

        // Atualiza todos os textos na tela
        foreach (var text in timerTexts)
        {
            if (text != null)
            {
                text.text = formattedTime;
            }
        }
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
