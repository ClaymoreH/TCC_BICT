using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FifoManager : PuzzleManager
{
    [Header("Configurações do Puzzle")]
    public List<Transform> slots;
    
    [Header("Referência ao Puzzle")]
    public Puzzle puzzle;

    [Header("Animação do Jogador")]
    public Animator playerAnimator;

    private void Start()
    {
        base.Start();
    }

    public override void ValidarPuzzle()
    {
        StartCoroutine(ValidateFIFO());
    }

private IEnumerator ValidateFIFO()
{
    if (audioSource != null && clickSound != null)
    {
        audioSource.PlayOneShot(clickSound);
    }

    yield return new WaitForSeconds(clickSound.length);

    List<PuzzleObjectData> objectsInSlots = new List<PuzzleObjectData>();

    foreach (Transform slot in slots)
    {
        if (slot.childCount > 0)
        {
            PuzzleObjectData objectData = slot.GetChild(0).GetComponent<PuzzleObjectData>();
            if (objectData != null)
            {
                objectsInSlots.Add(objectData);
            }
            else
            {
                ExibirFeedback("ERRO: Objeto inválido.", errorSound);
                yield break;
            }
        }
        else
        {
            ExibirFeedback("ERRO: Slot vazio.", errorSound);
            yield break;
        }
    }

    for (int i = 0; i < objectsInSlots.Count - 1; i++)
    {
        try
        {
            System.DateTime currentDateTime = System.DateTime.Parse($"{objectsInSlots[i].data} {objectsInSlots[i].hora}");
            System.DateTime nextDateTime = System.DateTime.Parse($"{objectsInSlots[i + 1].data} {objectsInSlots[i + 1].hora}");

            if (currentDateTime > nextDateTime)
            {
                ExibirFeedback("ERRO: A ordem dos processos está incorreta.", errorSound);
                yield break;
            }
        }
        catch (System.FormatException)
        {
            ExibirFeedback("ERRO: Formato inválido.", errorSound);
            yield break;
        }
    }

    ExibirFeedback("Sucesso! A ordem está correta.", successSound);

    // Destroi todos os objetos filhos do painel
    Transform panelTransform = puzzle.transform; 
    foreach (Transform child in panelTransform)
    {
        Destroy(child.gameObject);
    }

    if (puzzle != null)
    {
        puzzle.CompletePuzzle();
        PlayHappyAnimation(); // Chama a animação feliz

    }
}

}
