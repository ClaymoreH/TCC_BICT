using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Importar para usar o carregamento de cenas
using System.Collections.Generic;

public class LevelSelector : MonoBehaviour
{
    // Lista de imagens para selecionar (definidas no Inspetor)
    public Image[] imagens;

    // Lista de conjuntos de elementos a associar a cada imagem
    [System.Serializable]
    public class ConjuntoDeElementos
    {
        public GameObject[] elementos;
        public int sceneIndex; // Índice da cena a ser carregada
    }

    public ConjuntoDeElementos[] conjuntosDeElementos;

    private int imagemSelecionada = 0;

    // Fatores de escala para aumentar a imagem selecionada
    public Vector3 escalaNormal = new Vector3(1f, 1f, 1f);
    public Vector3 escalaAumentada = new Vector3(1.2f, 1.2f, 1f);

    // Parâmetros para o círculo
    public float raioCirculo = 150f; // Raio do círculo
    public float anguloInicial = 0f; // Posição inicial das imagens no círculo

    void Start()
    {
        // Inicializa a primeira imagem
        AtualizarSelecao();
        OrganizarImagensCircularmente();
    }

    void Update()
    {
        // Detecta a entrada de teclado para mudar entre as imagens
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MudarSelecao(1); // Avança para a próxima imagem
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MudarSelecao(-1); // Volta para a imagem anterior
        }

        // Detecta a tecla Enter para carregar a cena associada à imagem selecionada
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CarregarCenaSelecionada();
        }
    }

    // Método para alterar a imagem selecionada
    public void MudarSelecao(int direcao)
    {
        // Atualiza a seleção com base na direção
        imagemSelecionada = (imagemSelecionada + direcao + imagens.Length) % imagens.Length;
        AtualizarSelecao();
        OrganizarImagensCircularmente();
    }

    // Atualiza o tamanho da imagem selecionada e ativa/desativa os conjuntos de elementos
    void AtualizarSelecao()
    {
        // Redefine todas as imagens para o tamanho normal
        foreach (Image img in imagens)
        {
            img.transform.localScale = escalaNormal;
        }

        // Aumenta a imagem selecionada
        imagens[imagemSelecionada].transform.localScale = escalaAumentada;

        // Desativa todos os conjuntos de elementos
        foreach (var conjunto in conjuntosDeElementos)
        {
            foreach (var elemento in conjunto.elementos)
            {
                elemento.SetActive(false);
            }
        }

        // Ativa o conjunto de elementos correspondente à imagem selecionada
        if (imagemSelecionada >= 0 && imagemSelecionada < conjuntosDeElementos.Length)
        {
            foreach (var elemento in conjuntosDeElementos[imagemSelecionada].elementos)
            {
                elemento.SetActive(true);
            }
        }
    }

    // Organiza as imagens em um círculo
    void OrganizarImagensCircularmente()
    {
        int totalImagens = imagens.Length;
        float anguloEntreImagens = 360f / totalImagens; // Ângulo entre cada imagem no círculo

        // Posiciona cada imagem ao longo do círculo
        for (int i = 0; i < totalImagens; i++)
        {
            float anguloAtual = anguloInicial + i * anguloEntreImagens; // Calcula o ângulo para a posição da imagem
            Vector3 posicao = new Vector3(Mathf.Cos(anguloAtual * Mathf.Deg2Rad) * raioCirculo, Mathf.Sin(anguloAtual * Mathf.Deg2Rad) * raioCirculo, 0f);
            imagens[i].transform.localPosition = posicao; // Ajusta a posição local da imagem
        }
    }

    // Método para carregar a cena associada à imagem selecionada
    void CarregarCenaSelecionada()
    {
        if (imagemSelecionada >= 0 && imagemSelecionada < conjuntosDeElementos.Length)
        {
            int sceneIndex = conjuntosDeElementos[imagemSelecionada].sceneIndex;
            // Verifica se o índice da cena é válido
            if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                // Carrega a cena com base no índice
                SceneManager.LoadScene(sceneIndex);
            }
            else
            {
                Debug.LogWarning("Índice de cena inválido ou fora do alcance.");
            }
        }
    }
}
