using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public int id;
    public int status;        // 0 = inativo, 1 = ativo
    public int next_status;   
    public string[] lines; 
    public DialogueChoice[] choices; 
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public string actionType;
    public int ActionID;
    public string[] responseLines;   // Linhas de resposta associadas à escolha
}

[System.Serializable]
public class DialogueDatabase
{
    public DialogueData[] dialogues; 
}