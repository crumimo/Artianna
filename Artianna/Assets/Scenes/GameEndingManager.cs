using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndingManager : MonoBehaviour
{
    public static GameEndingManager Instance { get; private set; }

    public string goodEndingScene; // Название сцены для хорошей концовки
    public string badEndingScene; // Название сцены для плохой концовки
    public string finalSceneCheck; // Название сцены, после которой произойдет проверка

    private int goodChoicesCount = 0; // Количество хороших выборов
    private int badChoicesCount = 0; // Количество плохих выборов

    private void Awake()
    {
        // Убедимся, что объект единственный
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Не уничтожать объект при смене сцены
    }

    public void RegisterChoice(bool isGoodChoice)
    {
        if (isGoodChoice)
        {
            goodChoicesCount++;
        }
        else
        {
            badChoicesCount++;
        }

        Debug.Log($"Choice registered: {(isGoodChoice ? "Good" : "Bad")}. Good: {goodChoicesCount}, Bad: {badChoicesCount}");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Проверяем, совпадает ли текущая сцена с финальной сценой для проверки
        if (scene.name == finalSceneCheck)
        {
            Debug.Log("Final scene reached. Deciding ending...");
            DecideEnding();
        }
    }

    private void DecideEnding()
    {
        // Определяем, куда отправить игрока, основываясь на количестве выборов
        if (goodChoicesCount >= badChoicesCount)
        {
            Debug.Log($"Good ending chosen! Loading scene: {goodEndingScene}");
            SceneManager.LoadScene(goodEndingScene);
        }
        else
        {
            Debug.Log($"Bad ending chosen! Loading scene: {badEndingScene}");
            SceneManager.LoadScene(badEndingScene);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Получить количество выборов для отладки или других целей
    public (int good, int bad) GetChoiceCounts()
    {
        return (goodChoicesCount, badChoicesCount);
    }
}
