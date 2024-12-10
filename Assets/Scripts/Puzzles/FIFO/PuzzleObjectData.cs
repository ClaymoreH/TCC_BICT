using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PuzzleObjectData : MonoBehaviour
{
    [Header("Informações do Objeto")]
    public string objectName; 
    public string data;       
    public string hora;       
    public int prioridade;    
    public float tempoExecucao; 
    public float ordemChegada; 

    [TextArea]
    public string descricao;  // Descrição adicional do objeto (opcional)

    void Start()
    {
    }

    public void ExibirInformacoes()
    {
    }
}
