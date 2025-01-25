using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Necessário para manipular Image no Canvas

[CreateAssetMenu(fileName = "FIFODatabase", menuName = "Game/FIFO Database")]
public class FIFODatabase : ScriptableObject
{
    public List<FIFOData> alerts = new List<FIFOData>();
}

[System.Serializable]
public class FIFOData
{
    // Mantendo o tipo Sprite para arrastar e soltar no Editor
    public Sprite icon; // Ícone do alerta para o Canvas
    public Sprite iconDrag; // Ícone arrastável do alerta para o Canvas

    // Informações para o PuzzleObjectData
    public string name;  // Nome do alerta
    public string data;  // Data (ex: 2025-01-01)
    public string hora;  // Hora (ex: 14:30)

    // Informações para o DisplayTextManager
    public string titulotext;
    public string datatext;
    public string corpotext;
    public string logtext;
}

[CreateAssetMenu(fileName = "SJFDatabase", menuName = "Game/SJF Database")]
public class SJFDatabase : ScriptableObject
{
    public List<SJFData> alerts = new List<SJFData>();
}

[System.Serializable]
public class SJFData
{
    public Sprite icon; // Ícone do alerta para o Canvas
    public Sprite iconDrag; // Ícone arrastavel, contem o puzzleobjectdata e o drag and drop, assim como um button
    public Sprite button; // Ícone do segundo button.

    // Informações para o PuzzleObjectData
    public int tempoExecucao; 
    public int ordemChegada;

    // Informações para o DisplayTextManager
    public string titulotext;
    public string tempotext;
    public string chegadatext;
    public string descricaotext;
}

[CreateAssetMenu(fileName = "RRDatabase", menuName = "Game/RR Database")]
public class RRDatabase : ScriptableObject
{
    public List<RRData> alerts = new List<RRData>();
}

[System.Serializable]
public class RRData
{
    public Sprite iconDrag; // Ícone arrastável do alerta para o Canvas om o PuzzleObjectData

    // Informações para o PuzzleObjectData
    public int tempoExecucao;  
    public int ValorOriginal; 
    public int processo;

    // Informações para o DisplayTextManager
    public string titulotext;
    public string tempotext;
    public string chegadatext;
    public string descricaotext;
}
