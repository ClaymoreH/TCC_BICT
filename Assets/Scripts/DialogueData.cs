using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public int id;
    public string[] lines;

}

[System.Serializable]
public class DialogueDatabase
{
    public DialogueData[] dialogues;
}

