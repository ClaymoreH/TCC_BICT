using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    public string text;
    public int npc; // 0 = Jogador, 1 = NPC
}


[System.Serializable]
public class DialogueData
{
    public int id;
    public int status;
    public int next_status;

    public List<DialogueLine> lines;
    public List<DialogueChoice> choices;
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public string actionType;
    public int ActionID;
    public List<DialogueLine> responseLines;
}


[System.Serializable]
public class DialogueDatabase
{
    public DialogueData[] dialogues; 
}