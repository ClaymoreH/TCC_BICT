using UnityEngine;
using UnityEngine.Tilemaps; // Importa a biblioteca do Tilemap
using System.IO;

public class TilemapExporter : MonoBehaviour
{
    public Tilemap tilemap; // Referência ao seu Tilemap
    public Camera camera; // Câmera usada para renderizar
    public string fileName = "tilemap_export.png"; // Nome do arquivo de saída

    void Start()
    {
        ExportTilemap();
    }

    void ExportTilemap()
    {
        // Ajusta o aspecto da câmera para corresponder à resolução da tela
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        camera.aspect = aspectRatio; // Alinha o aspecto da câmera com a tela

        // Cria uma textura que tem o tamanho da tela
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        camera.targetTexture = renderTexture;
        camera.Render();

        // Salva a textura como uma imagem PNG
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // Converte a textura em bytes e salva em um arquivo PNG
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(Application.dataPath, fileName), bytes);

        // Limpa o que foi renderizado
        camera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
    }
}
