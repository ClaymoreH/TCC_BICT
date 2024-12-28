using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public GameObject questPanelPrefab; // Prefab do painel de cada quest
    public Transform questPanelParent;

    private QuestManager questManager;

    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>();
    }

    public void UpdateUI()
    {
        // Limpa a UI antiga
        foreach (Transform child in questPanelParent)
        {
            Destroy(child.gameObject);
        }

        // Atualiza com as quests ativas
        foreach (var quest in questManager.activeQuests)
        {
            var questPanel = Instantiate(questPanelPrefab, questPanelParent);
            questPanel.GetComponentInChildren<Text>().text = quest.questName;

            var objectivesParent = questPanel.transform.Find("Objectives");
            foreach (var objective in quest.objectives)
            {
                var objectiveText = new GameObject("ObjectiveText", typeof(Text)).GetComponent<Text>();
                objectiveText.text = $"{objective.description} - {(objective.isCompleted ? "Concluído" : "Pendente")}";
                objectiveText.transform.SetParent(objectivesParent);
            }
        }
    }
}
