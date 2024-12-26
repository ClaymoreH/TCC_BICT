using UnityEngine;

public class objetivo : MonoBehaviour
{
    // Propriedades
    public string ObjectiveDescription { get; private set; } // Descri��o do objetivo
    public bool IsComplete { get; private set; }

    // Inicializa o objetivo diretamente no Unity (via m�todo p�blico)
    public void Initialize(string description)
    {
        ObjectiveDescription = description; // Atribui a descri��o do objetivo
        IsComplete = false; // Inicialmente, o objetivo n�o est� completo
    }

    // M�todo para verificar se o objetivo foi completado
    public void CheckCompletion()
    {
        Debug.Log("Verificando se o objetivo foi completado.");
        // Aqui voc� pode implementar l�gica personalizada para verificar a conclus�o do objetivo
    }
}