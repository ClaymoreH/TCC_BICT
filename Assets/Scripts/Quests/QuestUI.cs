using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestUIManager : MonoBehaviour
{
    public GameObject questPanel;
    public GameObject questBanner; 
    public GameObject questItemPrefab;
    public Transform questListContainer; 

    public GameObject objectivePanel; 
    public TextMeshProUGUI questTitleText; 
    public TextMeshProUGUI questDescriptionText; 
    public Transform objectiveListContainer; 
    public GameObject objectiveItemPrefab;

    private List<GameObject> questItems = new List<GameObject>();
    private List<GameObject> objectiveItems = new List<GameObject>();

    private bool isQuestPanelVisible = false; 

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleQuestPanel();
        }
    }

    public void ToggleQuestPanel()
    {
        isQuestPanelVisible = !isQuestPanelVisible; // Alterna o estado
        questPanel.SetActive(isQuestPanelVisible); // Mostra ou oculta o painel
        questBanner.SetActive(isQuestPanelVisible); // Mostra ou oculta o painel
    }

    public void UpdateQuestUI(List<Quest> activeQuests, List<Quest> completedQuests)
    {
        // Limpa a lista anterior de missões
        foreach (var item in questItems)
        {
            Destroy(item);
        }
        questItems.Clear();

        // Exibe missões ativas
        foreach (var quest in activeQuests)
        {
            CreateQuestItem(quest);
        }

        // Exibe missões concluídas
        foreach (var quest in completedQuests)
        {
            CreateQuestItem(quest, true); // Indica que é uma missão concluída
        }
    }

    private void CreateQuestItem(Quest quest, bool isCompleted = false)
    {
        GameObject newItem = Instantiate(questItemPrefab, questListContainer);
        var text = newItem.GetComponentInChildren<TextMeshProUGUI>();

        if (text != null)
        {
            text.text = quest.questName; // Exibe apenas o título da missão

            // Estiliza missões concluídas
            if (isCompleted)
            {
                text.color = Color.gray; // Opcional: Colore o texto das missões concluídas
            }
        }

        // Adiciona o evento de clique para exibir os detalhes da missão no painel 2
        newItem.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => DisplayQuestDetails(quest));
        questItems.Add(newItem);
    }

    public void DisplayQuestDetails(Quest quest)
    {
        // Atualiza o título e a descrição da missão no painel 2
        questTitleText.text = quest.questName;
        questDescriptionText.text = quest.description;

        // Limpa a lista de objetivos anteriores
        foreach (var item in objectiveItems)
        {
            Destroy(item);
        }
        objectiveItems.Clear();

        // Adiciona os objetivos da missão ao painel
        foreach (var objective in quest.objectives)
        {
            CreateObjectiveItem(objective);
        }
    }

private void CreateObjectiveItem(QuestObjective objective)
{
    // Instancia o prefab para o Content do Scroll View
    GameObject newItem = Instantiate(objectiveItemPrefab, objectiveListContainer);
    var texts = newItem.GetComponentsInChildren<TextMeshProUGUI>();

    if (texts.Length >= 3)
    {
        texts[0].text = objective.title; // Título do objetivo
        texts[1].text = objective.description; // Descrição do objetivo
        texts[2].text = objective.isCompleted
            ? "Status: Concluído"
            : $"Status: {objective.requiredAmount} restantes"; // Status de conclusão
    }

    objectiveItems.Add(newItem);
}

}
