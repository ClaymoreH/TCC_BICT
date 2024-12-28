using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public int id;
    public int status;       // 0 = inativo, 1 = ativo
    public int next_status;  // Novo status após o diálogo ser chamado
    public string[] lines;

}

[System.Serializable]
public class DialogueDatabase
{
    public DialogueData[] dialogues;
}

