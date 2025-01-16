using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource; // Agora pode ser configurado diretamente no editor
    public AudioClip missionAddedSound;
    public AudioClip missionCompletedSound;
    public AudioClip objectiveCompletedSound;

    private void Awake()
    {
        // Se o AudioSource não for definido no editor, procura automaticamente
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("Nenhum AudioSource encontrado! Adicione um AudioSource ao GameObject ou atribua-o no editor.");
            }
        }
    }

    public void PlayMissionAddedSound()
    {
        if (audioSource != null && missionAddedSound != null)
        {
            audioSource.PlayOneShot(missionAddedSound);
        }
        else
        {
            Debug.LogWarning("MissionAddedSound ou AudioSource não configurados!");
        }
    }

    public void PlayMissionCompletedSound()
    {
        if (audioSource != null && missionCompletedSound != null)
        {
            audioSource.PlayOneShot(missionCompletedSound);
        }
        else
        {
            Debug.LogWarning("MissionCompletedSound ou AudioSource não configurados!");
        }
    }

    public void PlayObjectiveCompletedSound()
    {
        if (audioSource != null && objectiveCompletedSound != null)
        {
            audioSource.PlayOneShot(objectiveCompletedSound);
        }
        else
        {
            Debug.LogWarning("ObjectiveCompletedSound ou AudioSource não configurados!");
        }
    }
}
