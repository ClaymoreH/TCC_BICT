using UnityEngine;
using UnityEngine.UI;

public class Scroller : MonoBehaviour
{
    [SerializeField] private RawImage img; // A referência à RawImage
    [SerializeField] private float x;      // Velocidade no eixo X
    [SerializeField] private float y;      // Velocidade no eixo Y

    void Update()
    {
        // Atualiza o UV Rect da imagem para criar o efeito de rolagem
        img.uvRect = new Rect(
            img.uvRect.position + new Vector2(x, y) * Time.deltaTime, 
            img.uvRect.size
        );
    }
}
