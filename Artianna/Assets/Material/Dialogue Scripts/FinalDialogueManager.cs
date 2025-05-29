using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalDialogueManager : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public Dialogue finalDialogue;
    public string nextSceneName;

    private bool isFinalDialogueTriggered = false; // Флаг, чтобы финальный диалог запускался только один раз

    private void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();

        if (dialogueManager != null)
        {
            dialogueManager.AllDialoguesCompletedEvent += StartFinalDialogue;
        }

        // Если нет активных диалогов, проверяем вручную
        dialogueManager?.ForceCheckAllDialogues();
    }

    private void OnDestroy()
    {
        // Отписываемся от события, чтобы избежать утечек памяти
        if (dialogueManager != null)
        {
            dialogueManager.AllDialoguesCompletedEvent -= StartFinalDialogue;
        }
    }

    public void StartFinalDialogue()
    {
        // Проверяем, чтобы финальный диалог запускался только один раз
        if (isFinalDialogueTriggered || dialogueManager == null || !dialogueManager.AllDialoguesCompleted())
        {
            return;
        }

        isFinalDialogueTriggered = true; // Устанавливаем флаг
        Debug.Log("Запускаем финальный диалог...");
        dialogueManager.StartDialogue(finalDialogue, OnFinalDialogueEnd);
    }

    private void OnFinalDialogueEnd()
    {
        Debug.Log($"Финальный диалог завершён. Загружаем следующую сцену: {nextSceneName}");

        // Очищаем текущий диалог
        dialogueManager.EndDialogueInstantly(); // Новый метод для мгновенного завершения диалога

        // Начинаем переход на следующую сцену
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene(nextSceneName);
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}