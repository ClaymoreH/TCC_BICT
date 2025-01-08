using UnityEngine;
using TMPro;

public class ChoiceText : MonoBehaviour
{
    public System.Action onClickAction;
    private TMP_Text textComponent;
    private bool isSelected = false;

    public void SetAction(System.Action action, string choiceText)
    {
        onClickAction = action;
        textComponent = GetComponent<TMP_Text>();
        textComponent.text = choiceText;  // Aqui configuramos o texto
    }

    // Método para alterar a seleção visual
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        textComponent.color = isSelected ? Color.yellow : Color.white; // Altera a cor quando selecionado
    }

    private void OnMouseDown()
    {
        onClickAction?.Invoke(); // Invoca a ação associada ao clicar
    }
        // (Opcional) Efeitos visuais ao passar o mouse
    private void OnMouseEnter()
    {
        GetComponent<TMP_Text>().color = Color.yellow; // Altera a cor para indicar seleção
    }

    private void OnMouseExit()
    {
        GetComponent<TMP_Text>().color = Color.white; // Restaura a cor original
    }
}
