using UnityEngine;

public class AutoStartDialogue : MonoBehaviour
{
    public Dialogue dialogue; // Диалог, который будет начат автоматически
    private DialogueManager dialogueManager; // Ссылка на DialogueManager
    private bool dialogueTriggered = false; // Флаг для предотвращения повторного срабатывания

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();

        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager not found in the scene. AutoStartDialogue will not function.");
        }
        else
        {
            Debug.Log("AutoStartDialogue initialized. DialogueTriggered: " + dialogueTriggered);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!dialogueTriggered && other.CompareTag("Player"))
        {
            Debug.Log("AutoStartDialogue triggered by player.");
            dialogueTriggered = true; // Блокируем повторное срабатывание

            if (dialogueManager != null)
            {
                dialogueManager.StartDialogue(dialogue, OnDialogueEnd); // Начинаем диалог с обратным вызовом
            }
            else
            {
                Debug.LogError("DialogueManager is missing. Cannot start dialogue.");
            }
        }
    }

    private void OnDialogueEnd()
    {
        Debug.Log("AutoStartDialogue dialogue ended. Notifying DialogueManager.");

        if (dialogueManager != null)
        {
            // Уведомляем DialogueManager, что диалог завершён
            dialogueManager.RegisterDialogueComplete(null);

            // Принудительно уведомляем, если все диалоги завершены
            if (dialogueManager.AllDialoguesCompleted())
            {
                Debug.Log("All dialogues completed after AutoStartDialogue.");
                dialogueManager.OnAllDialoguesCompleted();
            }
            else
            {
                Debug.LogWarning("Some dialogues are still active after AutoStartDialogue.");
            }
        }
        else
        {
            Debug.LogError("DialogueManager reference lost.");
        }
    }
}
