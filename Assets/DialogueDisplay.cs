using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueDisplay : MonoBehaviour
{
    public Text dialogueText; // UI Text para exibir o di�logo
    public GameObject dialogueBox; // Painel que cont�m a UI do di�logo

    private Coroutine currentCoroutine;

    void Start()
    {
        dialogueBox.SetActive(false); // Oculta a caixa de di�logo inicialmente
    }

    public void ShowDialogue(string[] lines)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(DisplayLines(lines));
    }

    private IEnumerator DisplayLines(string[] lines)
    {
        dialogueBox.SetActive(true);

        foreach (string line in lines)
        {
            dialogueText.text = line;
            yield return new WaitForSeconds(2.5f); // Tempo para exibir cada linha
        }

        dialogueBox.SetActive(false);
    }
}

