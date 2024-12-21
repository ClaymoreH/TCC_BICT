using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GerenciadorDeDialogo : MonoBehaviour
{
    public GameObject painelDeDialogo;
    public Image imagemPerfil;
    public Text textoDialogo;
    public Text nomeDoObjeto;
    public float velocidadeDoTexto = 0.05f;

    private Coroutine corrotinaAtual;

    public void ExibirDialogo(Sprite spriteImagem, string dialogo, string nomeObjetoTexto)
    {
        painelDeDialogo.SetActive(true);
        imagemPerfil.sprite = spriteImagem;
        nomeDoObjeto.text = nomeObjetoTexto;

        if (corrotinaAtual != null)
            StopCoroutine(corrotinaAtual);

        corrotinaAtual = StartCoroutine(DigitarTexto(dialogo));
    }

    private IEnumerator DigitarTexto(string dialogo)
    {
        textoDialogo.text = "";
        foreach (char letra in dialogo.ToCharArray())
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadeDoTexto);
        }
    }

    public void FecharDialogo()
    {
        painelDeDialogo.SetActive(false);

        if (corrotinaAtual != null)
            StopCoroutine(corrotinaAtual);
    }
}
