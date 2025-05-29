using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public Canvas interactionCanvas; // Ссылка на Canvas, где будет отображаться подсказка
    public GameObject additionalObject; // Дополнительный объект, который появляется при взаимодействии
    public Dialogue dialogue; // Диалог для данного объекта
    public DialogueManager dialogueManager; // Ссылка на DialogueManager

    private bool isInRange = false; // Проверка, находится ли игрок в зоне триггера
    private bool hasInteracted = false; // Флаг, чтобы предотвратить повторное взаимодействие

    private void Start()
    {
        if (interactionCanvas != null)
        {
            interactionCanvas.gameObject.SetActive(false);
        }

        if (additionalObject != null)
        {
            additionalObject.SetActive(false); // Скрыть дополнительный объект
        }
    }

    private void Update()
    {
        if (isInRange && !hasInteracted && Input.GetKeyDown(KeyCode.F))
        {
            if (dialogueManager != null)
            {
                dialogueManager.StartDialogue(dialogue, EndInteraction);
                hasInteracted = true;
                interactionCanvas.gameObject.SetActive(false); // Скрыть подсказку
                if (additionalObject != null)
                {
                    additionalObject.SetActive(false); // Скрыть дочерний объект после взаимодействия
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
            if (!hasInteracted)
            {
                interactionCanvas.gameObject.SetActive(true);
                if (additionalObject != null)
                {
                    additionalObject.SetActive(true); // Показать дочерний объект при входе в зону
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            if (!hasInteracted)
            {
                interactionCanvas.gameObject.SetActive(false);
                if (additionalObject != null)
                {
                    additionalObject.SetActive(false);
                }
            }
        }
    }

    private void EndInteraction()
    {
        this.enabled = false;

        if (interactionCanvas != null)
        {
            interactionCanvas.gameObject.SetActive(false); // Скрыть подсказку
        }

        if (additionalObject != null)
        {
            additionalObject.SetActive(false); // Скрыть дочерний объект
        }

        // Уведомляем DialogueManager, что диалог завершен для этого объекта
        dialogueManager.RegisterDialogueComplete(this);
    }
}