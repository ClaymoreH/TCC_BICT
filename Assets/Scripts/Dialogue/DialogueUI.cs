﻿using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogueUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image dialogueImage;
    public TMP_Text dialogueText;
    public GameObject dialogueObject;
    public TMP_Text continueText;
    public float typingSpeed = 0.05f;
    public float blinkSpeed = 0.5f;
    public Image pressXImage;
    public TMP_Text pressXText;
    public GameObject choicesContainer;
    public GameObject choiceButtonPrefab;

    [Header("Audio Settings")]
    public AudioSource typingAudioSource; // Referência ao AudioSource
    public AudioClip typingSound;        // Som de digitação
    public AudioClip pressXSound;         // Som para o Press X

    private Coroutine blinkCoroutine;
    public GameObject choicePrefab;
    public List<ChoiceText> choiceTextList = new List<ChoiceText>();
    public int currentChoiceIndex = 0;
    public TMP_Text speakerNameText; // Nome do personagem que está falando


public void AddChoiceText(string choiceText, System.Action onClickAction)
{
    var choiceTextObject = Instantiate(choicePrefab, choicesContainer.transform);
    var choiceComponent = choiceTextObject.GetComponent<ChoiceText>();

    if (choiceComponent != null)
    {
        choiceComponent.SetAction(onClickAction, choiceText);  // Passando o texto da escolha
        choiceTextList.Add(choiceComponent);
    }

    // Inicialmente, nenhuma opção é selecionada
    choiceComponent.SetSelected(false);
}

    public void ShowChoices()
    {
        currentChoiceIndex = 0; // Reseta o índice
        UpdateChoiceSelection(); // Atualiza a seleção da escolha

        // Permite a navegação pelo teclado
        StartCoroutine(NavigateChoices());
    }

    private void UpdateChoiceSelection()
    {
        // Atualiza a seleção visual das escolhas
        for (int i = 0; i < choiceTextList.Count; i++)
        {
            choiceTextList[i].SetSelected(i == currentChoiceIndex);
        }
    }

    private System.Collections.IEnumerator NavigateChoices()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentChoiceIndex = (currentChoiceIndex > 0) ? currentChoiceIndex - 1 : choiceTextList.Count - 1;
                UpdateChoiceSelection();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentChoiceIndex = (currentChoiceIndex < choiceTextList.Count - 1) ? currentChoiceIndex + 1 : 0;
                UpdateChoiceSelection();
            }

            if (Input.GetKeyDown(KeyCode.Return)) // Seleção com Enter (ou qualquer outra tecla de sua escolha)
            {
                choiceTextList[currentChoiceIndex].SetSelected(false); // Desmarcar a escolha selecionada
                choiceTextList[currentChoiceIndex].onClickAction?.Invoke(); // Executa a ação da escolha
                break; // Sai da navegação de escolhas
            }

            yield return null;
        }
    }
    private readonly Dictionary<char, string> colorMap = new Dictionary<char, string>
    {
        { '#', "red" }, { '@', "blue" }, { '&', "green" }
    };

    public void InitializeUI()
    {
        dialogueObject.SetActive(false);
        SetUIElementState(false, pressXImage, pressXText);
    }

    private void SetUIElementState(bool state, params Graphic[] elements)
    {
        foreach (var element in elements)
            if (element != null) element.enabled = state;
    }

    public void ShowPressXMessage()
    {
        SetUIElementState(true, pressXImage, pressXText);
        PlayPressXSound();
    }
    public void HidePressXMessage() => SetUIElementState(false, pressXImage, pressXText);

    public void ShowContinueMessage()
    {
        if (continueText != null && blinkCoroutine == null)
            blinkCoroutine = StartCoroutine(BlinkText());
            continueText.gameObject.SetActive(true);
    }

    public void HideContinueMessage()
    {
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        blinkCoroutine = null;
        if (continueText != null) continueText.gameObject.SetActive(false);
    }

public IEnumerator TypeText(string line)
{
    dialogueText.text = string.Empty;

    for (int i = 0; i < line.Length; i++)
    {
        if (line[i] == '<' && line.IndexOf('>', i) is var endTag && endTag > 0)
        {
            // Adiciona a tag diretamente
            dialogueText.text += line.Substring(i, endTag - i + 1);
            i = endTag;
        }
        else
        {
            dialogueText.text += line[i];

            // Verifica se 'i' é divisível por 2 (ou seja, um índice par)
            if (i % 2 == 0)
            {
                PlayTypingSound(); // Toca o som
            }

            yield return new WaitForSeconds(typingSpeed);
        }
    }
}


    private void PlayTypingSound()
    {
        if (typingAudioSource != null && typingSound != null)
        {
            // Garante que o som seja reproduzido corretamente
            typingAudioSource.PlayOneShot(typingSound);
        }
    }
    private void PlayPressXSound()
    {
        if (typingAudioSource != null && pressXSound != null)
        {
            typingAudioSource.PlayOneShot(pressXSound);
        }
    }

    private IEnumerator BlinkText()
    {
        while (true)
        {
            continueText.alpha = 1f;
            yield return new WaitForSeconds(blinkSpeed);
            continueText.alpha = 0f;
            yield return new WaitForSeconds(blinkSpeed);
        }
    }

    public void AddChoiceButton(string choiceText, System.Action onClickAction)
    {
        var choiceButton = Instantiate(choiceButtonPrefab, choicesContainer.transform);
        choiceButton.GetComponentInChildren<TMP_Text>()?.SetText(choiceText);
        choiceButton.GetComponent<Button>()?.onClick.AddListener(() => onClickAction());
    }

    public string ProcessLine(string line)
    {
        var words = line.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length > 0 && colorMap.TryGetValue(words[i][0], out string color))
                words[i] = $"<color={color}>{words[i].Substring(1)}</color>";
        }
        return string.Join(" ", words);
    }
}