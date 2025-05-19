using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentObject : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float _transparencyValue = 0.7f;
    [SerializeField] private float _transparencyFadeTime = 0.4f;

    private SpriteRenderer _spriteRender;
    private SpriteRenderer playerSpriteRenderer;

    // Configuração para sorting order
    [SerializeField] private int sortingOrderBehind = 3; // Ordem atrás do objeto transparente
    [SerializeField] private int sortingOrderInFront = 7; // Ordem na frente do objeto transparente

    // Start is called before the first frame update
    void Awake()
    {
        _spriteRender = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se o objeto que entrou no trigger é o jogador
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            // Altera o sortingOrder do jogador para ficar atrás do objeto
            playerSpriteRenderer = collision.gameObject.GetComponent<SpriteRenderer>();
            if (playerSpriteRenderer != null)
            {
                playerSpriteRenderer.sortingOrder = sortingOrderBehind;
            }

            // Inicia o fade para deixar o objeto transparente
            StartCoroutine(FadeTree(_spriteRender, _transparencyFadeTime, _spriteRender.color.a, _transparencyValue));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Verifica se o objeto que saiu do trigger é o jogador
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null && playerSpriteRenderer != null)
        {
            // Restaura o sortingOrder do jogador para ele aparecer à frente do objeto
            playerSpriteRenderer.sortingOrder = sortingOrderInFront;
            
            // Restaura a transparência do objeto
            StartCoroutine(FadeTree(_spriteRender, _transparencyFadeTime, _spriteRender.color.a, 1f));
        }
    }

    private IEnumerator FadeTree(SpriteRenderer _spriteTransparency, float _fadeTime, float _startValue, float _targetTransparency)
    {
        float _timeElapsed = 0;
        while (_timeElapsed < _fadeTime)
        {
            _timeElapsed += Time.deltaTime;
            float _newAlpha = Mathf.Lerp(_startValue, _targetTransparency, _timeElapsed / _fadeTime);
            _spriteTransparency.color = new Color(_spriteTransparency.color.r, _spriteTransparency.color.g, _spriteTransparency.color.b, _newAlpha);
            yield return null;
        }
    }
}
