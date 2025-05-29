using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnding : MonoBehaviour
{
    public DialogueManager dialogueManager; // Ссылка на DialogueManager
    public string goodEndingScene;
    public string badEndingScene;

    public void CheckEnding()
    {
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager не назначен в GameEnding!");
            return;
        }

        // Получаем количество выборов из DialogueManager
        int goodChoices, badChoices;
        dialogueManager.GetChoiceCounts(out goodChoices, out badChoices);

        // Проверяем, куда отправить игрока
        if (goodChoices >= badChoices)
        {
            SceneManager.LoadScene(goodEndingScene);
        }
        else
        {
            SceneManager.LoadScene(badEndingScene);
        }
    }
}