using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI characterNameLeftText;
    public Image portraitLeftImage;
    public Image portraitRightImage;
    public TextMeshProUGUI dialogueText;
    public Image dialogueImage;

    public GameObject choicePanel;
    public TextMeshProUGUI goodChoiceButtonText;
    public TextMeshProUGUI badChoiceButtonText;
    public Button goodChoiceButton, badChoiceButton;

    public GameObject dialogueUI;
    public GameObject backgroundObject;

    private Queue<DialogueLine> linesQueue;
    private int goodChoicesCount = 0;
    private int badChoicesCount = 0;

    private bool isDialogueActive = false;
    private bool waitingForNextLine = false;
    private string nextText;
    
    private Dialogue currentDialogue; // Текущий диалог

    private System.Action onDialogueEnd;

    private static int activeDialogues = 0; // Счётчик активных диалогов
    private List<InteractableObject> interactables = new List<InteractableObject>(); // Список всех взаимодействуемых объектов на сцене

    private Sprite persistentDialogueImage = null; // Хранит выбранное изображение
    public event Action AllDialoguesCompletedEvent; // Событие для завершения всех диалогов
    
    public MonoBehaviour playerController; // Ссылка на скрипт управления игроком


    void Start()
    {
        activeDialogues = 0; // Сбрасываем активные диалоги
        Debug.Log("DialogueManager initialized. Active dialogues reset.");
    
        linesQueue = new Queue<DialogueLine>();
        choicePanel.SetActive(false);
        dialogueUI.SetActive(false);
        dialogueImage.gameObject.SetActive(false);
        portraitLeftImage.gameObject.SetActive(false);
        portraitRightImage.gameObject.SetActive(false);

        if (backgroundObject != null)
        {
            backgroundObject.SetActive(false);
        }

        interactables.AddRange(FindObjectsOfType<InteractableObject>());
        
        if (playerController != null)
        {
            playerController.enabled = false;
        }
    }

    void Update()
    {
        if (waitingForNextLine && Input.GetKeyDown(KeyCode.Space))
        {
            waitingForNextLine = false;
            DisplayNextLine();
        }
    }

    // Метод для начала диалога
    public void StartDialogue(Dialogue dialogue, System.Action onEnd = null)
    {
        if (isDialogueActive) return; // Уже активен, ничего не делаем.

        onDialogueEnd = onEnd;
        linesQueue.Clear();

        foreach (DialogueLine line in dialogue.lines)
        {
            linesQueue.Enqueue(line);
        }

        dialogueUI.SetActive(true);
        isDialogueActive = true;
        activeDialogues++;
        Debug.Log($"Dialogue started. Active Dialogues: {activeDialogues}");

        DisplayNextLine();
    }


    // Переход к следующей строке в диалоге
    public void DisplayNextLine()
    {
        if (linesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = linesQueue.Dequeue();

        characterNameLeftText.text = line.characterNameLeft;
        SetPortrait(portraitLeftImage, line.portraitLeft);
        SetPortrait(portraitRightImage, line.portraitRight);

        dialogueText.text = line.text;
        SetDialogueImage(line.dialogueImage);

        if (line.useBackground && backgroundObject != null)
        {
            backgroundObject.SetActive(true);
        }
        else if (!line.useBackground && backgroundObject != null)
        {
            backgroundObject.SetActive(false);
        }

        if (line.isChoice)
        {
            ShowChoices(line);
        }
        else
        {
            choicePanel.SetActive(false);
            waitingForNextLine = true;
        }
    }

    // Отображение портретов
    private void SetPortrait(Image portraitImage, Sprite sprite)
    {
        if (sprite != null)
        {
            portraitImage.sprite = sprite;
            portraitImage.gameObject.SetActive(true);
        }
        else
        {
            portraitImage.gameObject.SetActive(false);
        }
    }

    private void SetDialogueImage(Sprite image)
    {
        if (persistentDialogueImage != null)
        {
            dialogueImage.sprite = persistentDialogueImage; // Используем постоянное изображение
            dialogueImage.gameObject.SetActive(true);
        }
        else if (image != null)
        {
            dialogueImage.sprite = image;
            dialogueImage.gameObject.SetActive(true);
        }
        else
        {
            dialogueImage.gameObject.SetActive(false);
        }
    }

    private void ShowChoices(DialogueLine line)
    {
        choicePanel.SetActive(true);

        goodChoiceButtonText.text = line.goodChoiceText;
        badChoiceButtonText.text = line.badChoiceText;

        goodChoiceButton.onClick.RemoveAllListeners();
        badChoiceButton.onClick.RemoveAllListeners();

        goodChoiceButton.onClick.AddListener(() => HandleChoice(true, line.goodResultText, line.goodDialogueImage, line.goodPortraitLeft, line.additionalObject, line.backgroundObject));
        badChoiceButton.onClick.AddListener(() => HandleChoice(false, line.badResultText, line.badDialogueImage, line.badPortraitLeft, line.additionalObject, line.backgroundObject));
    }

    private void HandleChoice(bool isGood, string resultText, Sprite dialogueImageAfterChoice, Sprite newPortraitLeft, GameObject additionalObject, GameObject backgroundObject)
    {
        // Регистрация выбора в GameEndingManager
        if (GameEndingManager.Instance != null)
        {
            GameEndingManager.Instance.RegisterChoice(isGood);
        }
        else
        {
            Debug.LogError("GameEndingManager.Instance is null. Make sure GameEndingManager exists in the scene.");
        }

        // Увеличиваем счетчики выборов
        if (isGood)
            goodChoicesCount++;
        else
            badChoicesCount++;

        // Устанавливаем текст для следующей строки
        nextText = resultText;

        // Устанавливаем изображение, которое останется до конца диалога
        if (dialogueImageAfterChoice != null)
        {
            persistentDialogueImage = dialogueImageAfterChoice;
            dialogueImage.sprite = persistentDialogueImage;
            dialogueImage.gameObject.SetActive(true);
        }
        else if (persistentDialogueImage != null)
        {
            dialogueImage.sprite = persistentDialogueImage; // Сохраняем предыдущее изображение
            dialogueImage.gameObject.SetActive(true);
        }
        else
        {
            dialogueImage.gameObject.SetActive(false);
        }

        // Обновляем портрет после выбора
        if (newPortraitLeft != null)
        {
            portraitLeftImage.sprite = newPortraitLeft;
            portraitLeftImage.gameObject.SetActive(true);
        }

        // Активируем дополнительный объект, если он указан
        if (additionalObject != null)
        {
            additionalObject.SetActive(true);
        }

        // Активируем или скрываем фон, если указан
        if (backgroundObject != null)
        {
            backgroundObject.SetActive(true);
        }

        // Прячем панель выбора и отображаем результат выбора
        choicePanel.SetActive(false);
        dialogueText.text = resultText;
        waitingForNextLine = true;
    }
    public void EndDialogueInstantly()
    {
        // Мгновенно завершает диалог
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueUI.SetActive(false);
        currentDialogue = null;
    }
    private void EndDialogue()
    {
        dialogueUI.SetActive(false);
        isDialogueActive = false;

        if (activeDialogues > 0) activeDialogues--; // Защита от ухода в отрицательные значения.
        Debug.Log($"Dialogue ended. Active Dialogues: {activeDialogues}");

        persistentDialogueImage = null;

        if (activeDialogues <= 0 && interactables.Count == 0)
        {
            OnAllDialoguesCompleted();
        }
        if (playerController != null)
        {
            playerController.enabled = true; // Включаем управление игроком
        }

        onDialogueEnd?.Invoke();
    }
    public void ResetActiveDialogues()
    {
        activeDialogues = 0;
        Debug.Log("Active dialogues reset to 0.");
    }

    // Когда все диалоги завершены, вызываем финальный диалог
    public void OnAllDialoguesCompleted()
    {
        Debug.Log("Все диалоги завершены. Вызываем событие завершения всех диалогов.");
        AllDialoguesCompletedEvent?.Invoke(); // Вызываем событие
    }

    // Проверка завершения всех диалогов
    public bool AllDialoguesCompleted()
    {
        Debug.Log($"AllDialoguesCompleted check. Active dialogues: {activeDialogues}, Interactables: {interactables.Count}");
        return activeDialogues <= 0 && interactables.Count == 0;
    }

    // Регистрация завершённого диалога
    public void RegisterDialogueComplete(InteractableObject interactible)
    {
        if (interactible != null)
        {
            interactables.Remove(interactible);
        }

        Debug.Log($"RegisterDialogueComplete called. Interactables left: {interactables.Count}, Active Dialogues: {activeDialogues}");

        if (interactables.Count == 0 && activeDialogues <= 0)
        {
            OnAllDialoguesCompleted();
        }
    }

    // Новый метод для проверки активных диалогов
    public bool HasActiveDialogues()
    {
        Debug.Log($"HasActiveDialogues check. Active dialogues: {activeDialogues}, Interactables: {interactables.Count}");
        return activeDialogues > 0 || interactables.Count > 0;
    }
    
    public void ForceCheckAllDialogues()
    {
        if (activeDialogues <= 0 && interactables.Count == 0)
        {
            OnAllDialoguesCompleted();
        }
    }
    
    public void GetChoiceCounts(out int good, out int bad)
    {
        good = goodChoicesCount;
        bad = badChoicesCount;
    }
}


