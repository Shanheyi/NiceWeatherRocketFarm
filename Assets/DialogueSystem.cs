using System;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public GameObject continueHint;
    public KeyCode continueKey = KeyCode.Return;

    private string[] currentLines;
    private int currentLineIndex = 0;
    private bool isActive = false;

    public bool IsActive => isActive;

    public static event Action OnDialogueClosed;

    private void Awake()
    {
        Instance = this;
        dialoguePanel.SetActive(false);
    }

    public void ShowDialogue(string[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            return;
        }

        currentLines = lines;
        currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        continueHint.SetActive(true);
        isActive = true;
        Time.timeScale = 0f;
        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (currentLineIndex >= currentLines.Length)
        {
            CloseDialogue();
            return;
        }

        dialogueText.text = currentLines[currentLineIndex];
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        if (Input.GetKeyDown(continueKey))
        {
            currentLineIndex++;
            ShowCurrentLine();
        }
    }

    private void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        isActive = false;
        Time.timeScale = 1f;
        OnDialogueClosed?.Invoke();
    }
}
