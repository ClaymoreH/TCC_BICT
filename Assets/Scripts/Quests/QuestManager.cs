using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables; // Adicione esse using no topo

public class QuestManager : MonoBehaviour
{
    public QuestUIManager questUIManager;

    public List<Quest> activeQuests;
    public List<Quest> completedQuests;

    public NotificationManager notificationManager; // Arrastar no editor
    public AudioManager audioManager; // Arrastar no editor

    private void Start()
    {
        ResetQuests();
        completedQuests = new List<Quest>(); 
        questUIManager.UpdateQuestUI(activeQuests, completedQuests);

    }

    public void ResetQuests()
    {
        foreach (var quest in activeQuests)
        {
            foreach (var objective in quest.objectives)
            {
                objective.isCompleted = false; 
                objective.requiredAmount = objective.initialRequiredAmount; 
            }
        }
    }

public void AddQuest(Quest newQuest)
{
    if (newQuest != null && !activeQuests.Contains(newQuest) && !completedQuests.Contains(newQuest))
    {
        activeQuests.Add(newQuest);
        ResetQuests();

        // Exibir notificação e tocar som
        notificationManager?.ShowNotification($"Missão adicionada: {newQuest.questName}");
        audioManager?.PlayMissionAddedSound();

        Debug.Log($"Missão '{newQuest.questName}' adicionada à lista de missões ativas!");
        questUIManager.UpdateQuestUI(activeQuests, completedQuests);
    }
    else
    {
        Debug.LogWarning("Missão já está ativa, concluída ou é inválida.");
    }
}


    public void UpdateQuestProgress(int targetID, ObjectiveType type, int amount = 1)
    {
        List<Quest> questsToComplete = new List<Quest>();

        foreach (var quest in activeQuests)
        {
            bool questUpdated = false;

            for (int i = 0; i < quest.objectives.Count; i++)
            {
                var objective = quest.objectives[i];

                if (objective.type == type && objective.targetID == targetID && !objective.isCompleted)
                {
                    if (objective.isSequential && i > 0 && !quest.objectives[i - 1].isCompleted)
                    {
                        Debug.Log($"O objetivo '{objective.description}' só pode ser concluído após o anterior!");
                        break;
                    }

                    objective.requiredAmount -= amount;
                    if (objective.requiredAmount <= 0)
                    {
                        objective.requiredAmount = 0;
                        objective.isCompleted = true;
                        GrantObjectiveReward(objective);

                        // Exibir notificação e tocar som
                        notificationManager?.ShowNotification($"Objetivo concluído: {objective.title}");
                        audioManager?.PlayObjectiveCompletedSound();

                        Debug.Log($"Objetivo '{objective.description}' concluído!");
                    }


                    questUpdated = true;
                    questUIManager.DisplayQuestDetails(quest);
                    break;
                }
            }

            if (questUpdated && IsQuestComplete(quest))
            {
                questsToComplete.Add(quest);
            }
        }

        foreach (var quest in questsToComplete)
        {
            CompleteQuest(quest);
        }
    }

    public bool CanCompleteObjective(int targetID, ObjectiveType type)
    {
        foreach (var quest in activeQuests)
        {
            for (int i = 0; i < quest.objectives.Count; i++)
            {
                var objective = quest.objectives[i];

                if (objective.type == type && objective.targetID == targetID)
                {
                    if (objective.isSequential && i > 0 && !quest.objectives[i - 1].isCompleted)
                    {
                        return false; 
                    }

                    return !objective.isCompleted;
                }
            }
        }

        return false; 
    }

private void GrantObjectiveReward(QuestObjective objective)
{
    if (objective.rewardExperience > 0)
    {
        Debug.Log($"Ganhou {objective.rewardExperience} XP pelo objetivo '{objective.description}'!");
    }

    if (objective.rewardItem != null)
    {
        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager != null)
        {
            inventoryManager.AddItem(objective.rewardItem);
            Debug.Log($"Recebeu o item '{objective.rewardItem.itemName}' pelo objetivo '{objective.description}'!");
        }
    }

    // Atualizar o status do diálogo com base no statusID
    if (objective.statusID > 0)
    {
        InteractiveDialogueTrigger dialogueTrigger = FindObjectOfType<InteractiveDialogueTrigger>();
        if (dialogueTrigger != null)
        {
            dialogueTrigger.SetDialogueStatus(objective.statusID, 1); // Altera o status para 1 (ativo)
            Debug.Log($"O status do diálogo com ID {objective.statusID} foi alterado para ativo!");
        }
        else
        {
            Debug.LogWarning("InteractiveDialogueTrigger não encontrado na cena.");
        }
    }
}


    public bool IsQuestComplete(Quest quest)
    {
        foreach (var objective in quest.objectives)
        {
            if (!objective.isCompleted)
                return false;
        }
        return true;
    }


public void CompleteQuest(Quest quest)
{
    Debug.Log($"Missão '{quest.questName}' concluída!");

    if (quest.rewardItem != null)
    {
        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        inventoryManager?.AddItem(quest.rewardItem);
    }

    activeQuests.Remove(quest);
    completedQuests.Add(quest);

    notificationManager?.ShowNotification($"Missão concluída: {quest.questName}");
    audioManager?.PlayMissionCompletedSound();

    // ✅ TOCAR TIMELINE
    if (quest.timelineToPlay != null)
    {
        PlayableDirector director = FindObjectOfType<PlayableDirector>();
        if (director != null)
        {
            director.playableAsset = quest.timelineToPlay;
            director.Play();
            Debug.Log($"Timeline da missão '{quest.questName}' foi disparada.");
        }
        else
        {
            Debug.LogWarning("Nenhum PlayableDirector encontrado na cena para tocar a Timeline.");
        }
    }

    if (!string.IsNullOrEmpty(quest.dialoguePanelName))
    {
        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        GameObject panel = null;

        foreach (var t in allTransforms)
        {
            if (t.gameObject.name == quest.dialoguePanelName)
            {
                panel = t.gameObject;
                break;
            }
        }

        if (panel != null)
        {
            panel.SetActive(true);
            Debug.Log($"Painel de diálogo '{panel.name}' ativado para a missão '{quest.questName}'.");
        }
    }

    if (quest.nextQuest != null)
    {
        AddQuest(quest.nextQuest);
    }

    questUIManager.UpdateQuestUI(activeQuests, completedQuests);
}



}
