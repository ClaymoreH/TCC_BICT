using UnityEngine;

public class Interagivel : MonoBehaviour
{
    public Sprite imagemPerfil;
    public string dialogo;
    public string nomeDoObjeto;

    private GerenciadorDeDialogo gerenciadorDeDialogo;

    private void Start()
    {
        gerenciadorDeDialogo = FindObjectOfType<GerenciadorDeDialogo>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gerenciadorDeDialogo.ExibirDialogo(imagemPerfil, dialogo, nomeDoObjeto);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gerenciadorDeDialogo.FecharDialogo();
        }
    }
}
