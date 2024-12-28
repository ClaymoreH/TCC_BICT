using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public List<Quest> activeQuests;

    public void UpdateQuestProgress(int targetID, ObjectiveType type, int amount = 1)
    {
        List<Quest> completedQuests = new List<Quest>();

        foreach (var quest in activeQuests)
        {
            bool questUpdated = false;

            foreach (var objective in quest.objectives)
            {
                if (objective.type == type && objective.targetID == targetID && !objective.isCompleted)
                {
                    if (type == ObjectiveType.DeliverItem)
                    {
                        objective.requiredAmount -= amount;
                        if (objective.requiredAmount <= 0)
                        {
                            objective.requiredAmount = 0;
                            objective.isCompleted = true;
                            Debug.Log($"Objetivo '{objective.description}' concluído!");
                        }
                    }

                    questUpdated = true;
                    break;
                }
            }

            if (questUpdated && IsQuestComplete(quest))
            {
                completedQuests.Add(quest);
            }
        }

        // Concluir missões completas
        foreach (var quest in completedQuests)
        {
            CompleteQuest(quest);
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
        Debug.Log($"Missão '{quest.questName}' concluída! Recompensa: {quest.rewardExperience} XP e {(quest.rewardItem != null ? quest.rewardItem.itemName : "nenhum item")}!");

        if (quest.rewardItem != null)
        {
            InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
            if (inventoryManager != null)
            {
                inventoryManager.AddItem(quest.rewardItem);
            }
        }

        activeQuests.Remove(quest);
    }

}
