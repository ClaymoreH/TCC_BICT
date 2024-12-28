using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundElementSpawner : MonoBehaviour
{
    public GameObject[] elements; // Array de prefabs (planetas, meteoros, etc.)
    public float spawnInterval = 5f; // Tempo entre spawns
    public Vector2 spawnXRange = new Vector2(10f, 15f); // Range no eixo X (começando da direita)
    public Vector2 spawnYRange = new Vector2(-5f, 5f); // Intervalo de altura (Y) onde o elemento pode aparecer
    public float elementSpeedX = 2f; // Velocidade no eixo X dos elementos
    public float elementSpeedY = 2f; // Velocidade no eixo Y dos elementos

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        // Spawnar elemento
        if (timer >= spawnInterval)
        {
            SpawnElement();
            timer = 0f;
        }
    }

    void SpawnElement()
    {
        // Escolhe um elemento aleatório
        GameObject element = elements[Random.Range(0, elements.Length)];

        // Determina a posição inicial do elemento (fora da tela à direita e com altura aleatória)
        Vector3 spawnPosition = new Vector3(
            Random.Range(spawnXRange.x, spawnXRange.y), // Posição X aleatória dentro do range (à direita)
            Random.Range(spawnYRange.x, spawnYRange.y), // Posição Y aleatória dentro do range
            1f       // Posição Z fixa (1)
        );

        // Instancia o elemento
        GameObject spawnedElement = Instantiate(element, spawnPosition, Quaternion.identity);
        StartCoroutine(MoveElement(spawnedElement));
    }

    IEnumerator MoveElement(GameObject element)
    {
        // Movimento livre no eixo X e Y
        while (element != null)
        {
            // Movimento no eixo X (da direita para a esquerda)
            element.transform.position += Vector3.left * elementSpeedX * Time.deltaTime;

            // Movimento no eixo Y (subindo ou descendo aleatoriamente)
            element.transform.position += Vector3.up * Mathf.Sin(Time.time * elementSpeedY) * Time.deltaTime;

            // Remove o elemento se sair da tela (quando ele estiver à esquerda da tela)
            if (element.transform.position.x < -10f) // Ajuste -10f conforme necessário para a largura da tela
            {
                Destroy(element);
            }

            yield return null;
        }
    }
}
