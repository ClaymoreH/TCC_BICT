using UnityEngine;

public class objetivo : MonoBehaviour
{
    // Propriedades
    public string ObjectiveDescription { get; private set; } // Descrição do objetivo
    public bool IsComplete { get; private set; }

    // Inicializa o objetivo diretamente no Unity (via método público)
    public void Initialize(string description)
    {
        ObjectiveDescription = description; // Atribui a descrição do objetivo
        IsComplete = false; // Inicialmente, o objetivo não está completo
    }

    // Método para verificar se o objetivo foi completado
    public void CheckCompletion()
    {
        Debug.Log("Verificando se o objetivo foi completado.");
        // Aqui você pode implementar lógica personalizada para verificar a conclusão do objetivo
    }
}