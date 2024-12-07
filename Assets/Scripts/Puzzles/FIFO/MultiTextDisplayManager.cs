using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiTextDisplayManager : MonoBehaviour
{
    [Header("Referências de Texto")]
    public List<Text> uiTextAreas;
    public List<TMP_Text> tmpTextAreas;

    [Header("Configuração de Botões e Textos")]
    public List<Button> buttons;
    public List<TextContent> buttonContents;

    [Header("Configurações de Áudio")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip textChangeSound;

    [Header("Configurações de Typewriter")]
    public float typeSpeed = 0.0f;
    public AudioClip typingSound;

    private Coroutine currentTypingCoroutine; // Coroutine para controlar o efeito de digitação.
    private AudioSource typingAudioSource;


    private void Start()
    {
        // Adiciona listeners aos botões para chamar OnButtonClicked quando clicados.
        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnButtonClicked(index));
        }

        typingAudioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnButtonClicked(int index)
    {
        // Reproduz o som de clique do botão.
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

        // Para a coroutine de digitação anterior
        if (currentTypingCoroutine != null)
        {
            StopCoroutine(currentTypingCoroutine);
        }

        currentTypingCoroutine = StartCoroutine(TypeOutText(buttonContents[index]));
    }
    private IEnumerator TypeOutText(TextContent content)
    {
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

        PlayTypingSound();

        // Define o tamanho do chunk de texto a ser exibido por frame ("suavidade" da digitação).
        int chunkSize = 3; 

        for (int i = 0; i < uiTextAreas.Count && i < content.uiTexts.Count; i++)
        {
            string text = content.uiTexts[i];
            for (int j = 0; j < text.Length; j += chunkSize)
            {
                int endIndex = Mathf.Min(j + chunkSize, text.Length);
                uiTextAreas[i].text += text.Substring(j, endIndex - j);
                yield return new WaitForSeconds(typeSpeed);
            }
        }

        for (int i = 0; i < tmpTextAreas.Count && i < content.tmpTexts.Count; i++)
        {
            string text = content.tmpTexts[i];
            for (int j = 0; j < text.Length; j += chunkSize)
            {
                int endIndex = Mathf.Min(j + chunkSize, text.Length);
                tmpTextAreas[i].text += text.Substring(j, endIndex - j);
                yield return new WaitForSeconds(typeSpeed);
            }
        }

        if (typingAudioSource != null && typingAudioSource.isPlaying)
        {
            typingAudioSource.Stop();
        }

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
}

[System.Serializable]
public class TextContent
{
    public List<string> uiTexts;
    public List<string> tmpTexts;
}