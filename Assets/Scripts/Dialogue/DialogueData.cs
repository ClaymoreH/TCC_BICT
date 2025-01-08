using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public int id;
    public int status;        // 0 = inativo, 1 = ativo
    public int next_status;   // Novo status após o diálogo ser chamado
    public string[] lines;    // Linhas de diálogo
    public DialogueChoice[] choices; // Escolhas associadas ao diálogo
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;        // Texto exibido para a escolha
    public string actionType;        // Tipo de ação a ser realizada (como string)
    public int ActionID;               // ID do item (caso aplicável)
}

[System.Serializable]
public class DialogueDatabase
{
    public DialogueData[] dialogues; // Lista de diálogos
}
