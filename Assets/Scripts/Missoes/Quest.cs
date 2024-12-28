using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    public string description;

    public List<QuestObjective> objectives;

    public int rewardExperience;
    public Item rewardItem;
}

[System.Serializable]
public class QuestObjective
{
    public string description;
    public ObjectiveType type;
    public int requiredAmount; 
    public int targetID;
    public bool isCompleted;
}

public enum ObjectiveType
{
    CollectItem,
    ReachLocation,
    InteractWithObject,
    DeliverItem,
}
