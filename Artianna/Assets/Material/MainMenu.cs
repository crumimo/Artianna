using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Метод для загрузки сцены
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Метод для выхода из игры
    public void ExitGame()
    {
        #if UNITY_EDITOR
        // Если игра запущена в редакторе Unity
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // Если игра запущена как сборка
        Application.Quit();
        #endif
    }
}
