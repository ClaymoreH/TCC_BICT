using System.Collections.Generic;

public class Quest
{
    public string QuestName { get; private set; }
    public string Description { get; private set; }
    public List<QuestObjective> Objectives { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsComplete { get; private set; }

    public Quest(string questName, string description)
    {
        QuestName = questName;
        Description = description;
        Objectives = new List<QuestObjective>();
        IsActive = false;
        IsComplete = false;
    }

    public void Activate()
    {
        IsActive = true;
        // Notificar UI
    }

    public void Complete()
    {
        IsComplete = true;
        // Notificar UI
    }

    public void UpdateObjectives()
    {
        foreach (var objective in Objectives)
        {
            if (!objective.IsComplete)
            {
                objective.CheckCompletion();
            }
        }

        if (Objectives.TrueForAll(o => o.IsComplete))
        {
            Complete();
        }
    }
}