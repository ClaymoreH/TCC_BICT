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
    public Color normalColor = Color.white; // Cor padrão do texto
    public Color warningColor = Color.red;  // Cor ao reduzir o tempo
    public float colorChangeDuration = 1f;  // Duração do vermelho

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
                text.color = normalColor; // Reseta a cor para o normal
            }
        }
    }

    public void ReduceTime(int secondsToReduce)
    {
        // Reduz o tempo e limita para no mínimo 0
        currentValue = Mathf.Max(0, currentValue - secondsToReduce);

        // Atualiza o UI imediatamente
        UpdateTimerUI();

        // Altera a cor dos textos para vermelho temporariamente
        foreach (var text in timerTexts)
        {
            if (text != null)
            {
                text.color = warningColor; // Define como vermelho
            }
        }

        // Retorna à cor normal após um tempo
        Invoke(nameof(ResetTextColor), colorChangeDuration);

        // Verifica se o tempo chegou a 0
        if (currentValue == 0)
        {
            CancelInvoke(nameof(DecrementTimer)); 
            TriggerGameOver();
        }
    }

    void ResetTextColor()
    {
        // Restaura a cor normal para todos os textos
        foreach (var text in timerTexts)
        {
            if (text != null)
            {
                text.color = normalColor;
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
