using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public QuestUIManager questUIManager;

    public List<Quest> activeQuests;
    public List<Quest> completedQuests;

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
            Debug.Log($"Missão '{newQuest.questName}' adicionada à lista de missões ativas!");
            ResetQuests();
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

        // Recompensa e outras lógicas
        if (quest.rewardItem != null)
        {
            InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
            if (inventoryManager != null)
            {
                inventoryManager.AddItem(quest.rewardItem);
            }
        }

        activeQuests.Remove(quest);
        completedQuests.Add(quest);

        // Adicionar a próxima missão, se houver
        if (quest.nextQuest != null)
        {
            AddQuest(quest.nextQuest);
        }

        questUIManager.UpdateQuestUI(activeQuests, completedQuests); // Atualiza a interface
    }

}
