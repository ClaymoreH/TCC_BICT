using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PuzzleObjectData : MonoBehaviour
{
    [Header("Informações do Objeto")]
    public string objectName; // Nome do objeto
    public string data;       // Data associada ao objeto
    public string hora;       // Hora associada ao objeto
    public int prioridade;    // Prioridade do objeto
    public float tempoExecucao; // Tempo de execução

    [TextArea]
    public string descricao;  // Descrição adicional do objeto (opcional)

    void Start()
    {
    }

    public void ExibirInformacoes()
    {
    }
}
