using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public float speedX; // Velocidade no eixo X
    public float speedY; // Velocidade no eixo Y

    [SerializeField]
    private Renderer bgRenderer;

    public bool moveX = true; // Controle de movimento no eixo X
    public bool moveY = true; // Controle de movimento no eixo Y

    void Update()
    {
        // Calcula o deslocamento com base nas opções de movimento
        float offsetX = moveX ? speedX * Time.deltaTime : 0;
        float offsetY = moveY ? speedY * Time.deltaTime : 0;

        // Aplica o deslocamento no material
        bgRenderer.material.mainTextureOffset += new Vector2(offsetX, offsetY);
    }
}
