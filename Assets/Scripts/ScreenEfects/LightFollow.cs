using UnityEngine;

public class LightFollow : MonoBehaviour
{
    public RectTransform lightMask; // A máscara circular
    public Transform player;        // O jogador
    public Camera mainCamera;       // A câmera principal

    void Update()
    {
        if (lightMask != null && player != null && mainCamera != null)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(player.position);
            lightMask.position = screenPos; // Alinha a máscara com o jogador
        }
    }
}
