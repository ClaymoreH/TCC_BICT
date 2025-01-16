using System.Collections;
using UnityEngine;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    public GameObject notificationPrefab; // Prefab do texto de notificação
    public Transform notificationContainer; // Contêiner para exibir as notificações
    public float displayTime = 3f; // Tempo de exibição do texto

    public void ShowNotification(string message)
    {
        GameObject notification = Instantiate(notificationPrefab, notificationContainer);
        var textComponent = notification.GetComponentInChildren<TextMeshProUGUI>();

        if (textComponent != null)
        {
            textComponent.text = message;
        }

        Destroy(notification, displayTime); // Remove o texto após o tempo especificado
    }
}
