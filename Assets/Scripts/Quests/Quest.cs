using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    public string description;

    public List<QuestObjective> objectives;

    public int rewardExperience;
    public Item rewardItem;

    public Quest nextQuest;

    public string dialoguePanelName;

    public PlayableAsset timelineToPlay; // <- Timeline opcional ao concluir a missão

}



[System.Serializable]
public class QuestObjective
{
    public string title;
    public string description;
    public ObjectiveType type;
    public int requiredAmount;
    public int targetID;
    public bool isCompleted;
    public int initialRequiredAmount; // Valor inicial para resetar
    public bool isSequential; // Indica se o objetivo depende do anterior
    public int statusID; // Numero do Id para definir o status como 1 o dialogo apos completar o objetivo
    public int rewardExperience;
    public Item rewardItem;

    public QuestObjective nextObjective;

    public void Initialize()
    {
        initialRequiredAmount = requiredAmount; // Salva o valor inicial
    }
}


public enum ObjectiveType
{
    CollectItem,
    ReachLocation,
    InteractWithObject,
    DeliverItem,
    SolvePuzzle,
}
