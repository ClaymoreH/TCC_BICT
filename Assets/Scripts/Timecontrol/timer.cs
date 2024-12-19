using UnityEngine;
using TMPro;

public class timer : MonoBehaviour
{
    public int startValue = 360; // Valor inicial do timer
    private int currentValue;    // Valor atual do timer

    public TextMeshProUGUI timerText; // Referência ao TextMeshPro no Canvas

    void Start()
    {
        // Certifique-se de que o TextMeshPro está configurado
        if (timerText == null)
        {
            Debug.LogError("TimerText não está atribuído no Inspector!");
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
        }
    }

    void UpdateTimerUI()
    {
        timerText.text = currentValue.ToString(); // Atualiza o texto com o valor atual
    }
}
