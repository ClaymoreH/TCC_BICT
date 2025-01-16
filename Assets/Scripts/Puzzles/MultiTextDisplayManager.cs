using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MultiTextDisplayManager : MonoBehaviour
{
    [Header("Referências de Texto")]
    public List<Text> uiTextAreas; // Áreas de texto para UI Text
    public List<TMP_Text> tmpTextAreas; // Áreas de texto para TextMeshPro

    [Header("Configuração de Botões e Textos")]
    public List<Button> buttons; // Botões para interação
    public List<TextContent> buttonContents; // Conteúdos associados aos botões

    [Header("Configurações de Áudio")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip textChangeSound;

    [Header("Configurações de Typewriter")]
    public float typeSpeed = 0.05f; // Velocidade do efeito de digitação
    public int chunkSize = 5; // Tamanho do bloco de texto para digitação
    public float elementInterval = 0.5f; // Intervalo entre cada elemento de texto
    public AudioClip typingSound;

    private Coroutine currentTypingCoroutine;
    private AudioSource typingAudioSource;

    private void Start()
    {
        for (int buttonIndex = 0; buttonIndex < buttons.Count; buttonIndex++)
        {
            int index = buttonIndex; // Captura o índice do botão
            buttons[index].onClick.AddListener(() => HandleButtonClick(index));
        }

        typingAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public void HandleButtonClick(int buttonIndex)
    {
        foreach (var content in buttonContents)
        {
            if (content.associatedButtonIndices.Contains(buttonIndex))
            {
                OnButtonClicked(buttonContents.IndexOf(content));
                return;
            }
        }

        Debug.LogWarning("Nenhum conteúdo associado a este botão!");
    }

    private void OnButtonClicked(int index)
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }

        if (typingAudioSource != null && typingAudioSource.isPlaying)
        {
            typingAudioSource.Stop();
        }

        UpdateTexts(index);
    }

    private void UpdateTexts(int index)
    {
        if (index < 0 || index >= buttonContents.Count)
        {
            Debug.LogWarning("Índice fora do alcance!");
            return;
        }

        if (currentTypingCoroutine != null)
        {
            StopCoroutine(currentTypingCoroutine);
        }

        currentTypingCoroutine = StartCoroutine(TypeOutText(buttonContents[index]));
    }

    private IEnumerator TypeOutText(TextContent content)
    {
        // Limpa todos os textos antes de iniciar a digitação
        foreach (var textArea in uiTextAreas)
        {
            textArea.text = "";
        }
        foreach (var textArea in tmpTextAreas)
        {
            textArea.text = "";
        }

        for (int i = 0; i < content.uiTexts.Count; i++)
        {
            content.uiTexts[i] = content.uiTexts[i].Replace("\\n", "\n");
        }

        for (int i = 0; i < content.tmpTexts.Count; i++)
        {
            content.tmpTexts[i] = content.tmpTexts[i].Replace("\\n", "\n");
        }

        if (audioSource != null && buttonClickSound != null)
        {
            yield return new WaitForSeconds(buttonClickSound.length);
        }

        // Digitação para UI Text
        for (int i = 0; i < uiTextAreas.Count && i < content.uiTexts.Count; i++)
        {
            PlayTypingSound();
            string text = content.uiTexts[i];
            for (int j = 0; j < text.Length; j += chunkSize)
            {
                int endIndex = Mathf.Min(j + chunkSize, text.Length);
                uiTextAreas[i].text += text.Substring(j, endIndex - j);
                yield return new WaitForSeconds(typeSpeed);
            }
            StopTypingSound(); // Para o som ao final do elemento
            yield return new WaitForSeconds(elementInterval); // Espera entre elementos
        }

        // Digitação para TMP_Text
        for (int i = 0; i < tmpTextAreas.Count && i < content.tmpTexts.Count; i++)
        {
            PlayTypingSound();
            string text = content.tmpTexts[i];
            for (int j = 0; j < text.Length; j += chunkSize)
            {
                int endIndex = Mathf.Min(j + chunkSize, text.Length);
                tmpTextAreas[i].text += text.Substring(j, endIndex - j);
                yield return new WaitForSeconds(typeSpeed);
            }
            StopTypingSound(); // Para o som ao final do elemento
            yield return new WaitForSeconds(elementInterval); // Espera entre elementos
        }

        StopTypingSound(); // Garantia de parar o som ao final
        if (audioSource != null && textChangeSound != null)
        {
            audioSource.PlayOneShot(textChangeSound);
        }
    }

    private void PlayTypingSound()
    {
        if (typingSound != null && typingAudioSource != null)
        {
            typingAudioSource.loop = true;
            typingAudioSource.clip = typingSound;
            if (!typingAudioSource.isPlaying)
            {
                typingAudioSource.Play();
            }
        }
    }

    private void StopTypingSound()
    {
        if (typingAudioSource != null && typingAudioSource.isPlaying)
        {
            typingAudioSource.Stop();
        }
    }
}

[System.Serializable]
public class TextContent
{
    public List<int> associatedButtonIndices; // Índices dos botões associados a este conteúdo
    public List<string> uiTexts; // Textos para UI Text
    public List<string> tmpTexts; // Textos para TMP_Text
}
