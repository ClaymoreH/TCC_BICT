using UnityEngine;

public class TogglePanel : MonoBehaviour
{
    public GameObject panel; // Arraste seu painel no Inspector
    public KeyCode toggleKey = KeyCode.P; // Tecla para alternar a visibilidade

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
        Invoke(nameof(ShowGame), 2f); // Aguarda 2 segundos antes de mostrar a tela de Game Over
        }
    }


    void ShowGame()
    {
        panel.SetActive(true); // Ativa o Canvas de Game Over
    }
}
