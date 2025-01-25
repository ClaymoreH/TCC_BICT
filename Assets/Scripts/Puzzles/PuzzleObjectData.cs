using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PuzzleObjectData : MonoBehaviour
{
    [Header("Informações do Objeto")]
    public string name; 
    public string data;       
    public string hora;       
    public int prioridade;    
    public int tempoExecucao; 
    public int tempoExecucaoTotal; 
    public int ValorOriginal;
    public int ordemChegada; 
    public int processo;
    public int dropzoneID; // ID da DropZone a qual o objeto pertence
    public int PainelPuzzle;
    public GameObject Painel; // Referência ao painel

    [TextArea]
    public string descricao; 

    void Start()
    {
    }

    public void ExibirInformacoes()
    {
    }
}
