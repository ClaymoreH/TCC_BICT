using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public int startValue = 360; // Valor inicial do timer
    private int currentValue;    // Valor atual do timer

    public TextMeshProUGUI timerText; // Referência ao TextMeshPro no Canvas
    public Animator playerAnimator;  // Referência ao Animator do Player
    public GameObject gameOverScreen; // Referência à tela de Game Over (Canvas)

    void Start()
    {
        // Certifique-se de que o TextMeshPro está configurado
        if (timerText == null)
        {
            Debug.LogError("TimerText não está atribuído no Inspector!");
            return;
        }

        if (playerAnimator == null)
        {
            Debug.LogError("PlayerAnimator não está atribuído no Inspector!");
            return;
        }

        if (gameOverScreen == null)
        {
            Debug.LogError("GameOverScreen não está atribuído no Inspector!");
            return;
        }

        currentValue = startValue; // Define o valor inicial
        UpdateTimerUI(); // Atualiza a interface inicialmente
        InvokeRepeating(nameof(DecrementTimer), 1f, 1f); // Chama DecrementTimer a cada 1 segundo
    }

    void DecrementTimer()
    {
        if (currentValue > 0)
        {
            currentValue--; // Decrementa o valor do timer
            UpdateTimerUI(); // Atualiza a interface
        }
        else
        {
            CancelInvoke(nameof(DecrementTimer)); // Para o timer quando atingir 0
            TriggerGameOver(); // Ativa a lógica de Game Over
        }
    }

    void UpdateTimerUI()
    {
        timerText.text = currentValue.ToString(); // Atualiza o texto com o valor atual
    }

    void TriggerGameOver()
    {
        // Ativa o trigger "dead" no Animator
        playerAnimator.SetTrigger("Dead");

        // Ativa a tela de Game Over após um pequeno atraso para garantir que a animação seja exibida
        Invoke(nameof(ShowGameOverScreen), 2f); // Aguarda 2 segundos antes de mostrar a tela de Game Over
    }

    void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true); // Ativa o Canvas de Game Over
    }
}


    