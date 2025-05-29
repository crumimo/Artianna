using UnityEngine;
using UnityEngine.Video;

public class VideoToSceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneToLoad; // Название следующей сцены
    [SerializeField] private KeyCode skipKey = KeyCode.Space; // Клавиша для пропуска видео

    private VideoPlayer videoPlayer;

    private void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer == null)
        {
            Debug.LogError("На объекте отсутствует компонент VideoPlayer!");
            return;
        }

        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void Update()
    {
        if (Input.GetKeyDown(skipKey))
        {
            LoadNextScene();
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        // Используем SceneTransitionManager для перехода
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("SceneTransitionManager не найден. Загружаем сцену напрямую.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
        }
    }
}