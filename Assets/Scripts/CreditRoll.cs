using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CreditRollTMP : MonoBehaviour
{
    public TextMeshProUGUI creditTextPrefab; // Prefab com componente TextMeshProUGUI
    public Transform contentParent; // Conteúdo dentro do painel (Scroll View ou painel vertical)
    public float scrollSpeed = 30f;

    private List<GameObject> creditLines = new List<GameObject>();

    void Start()
    {
        LoadCredits();
    }

    void LoadCredits()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("credits");
        if (jsonFile != null)
        {
            CreditData creditData = JsonUtility.FromJson<CreditData>(jsonFile.text);
            foreach (string line in creditData.credits)
            {
                var textObj = Instantiate(creditTextPrefab, contentParent);
                textObj.text = line;
                creditLines.Add(textObj.gameObject);
            }
        }
        else
        {
            Debug.LogError("Arquivo credits.json não encontrado em Resources.");
        }
    }

    void Update()
    {
        contentParent.transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);

        foreach (var line in creditLines)
        {
            if (line.transform.position.y > Screen.height + 100)
                Destroy(line);
        }
    }

    [System.Serializable]
    public class CreditData
    {
        public List<string> credits;
    }
}
