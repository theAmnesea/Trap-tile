using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance = null;

    public bool m_isSplashScreen = false;

    [HideInInspector]
    public bool m_isGamePlayPaused = false;

    public GameObject m_crossFadeAnimation;

    [Header("For Levels Only")]
    public GameObject m_gameplayPausedMenu;
    public GameObject m_gameLost;

    public static string m_levelToLoad;

    private readonly float m_autoLoadLevelTime = 2f;

    private bool m_isSceneLoading = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;
        Application.targetFrameRate = 120;

        if (m_isSplashScreen)
        {
            m_levelToLoad = "01b Menu";
            StartCoroutine(LoadYourAsyncScene(m_autoLoadLevelTime));
        }
    }

    public void BackButtonPressed()
    {
        if (m_gameplayPausedMenu != null)
        {
            OnGameplayPaused(!m_gameplayPausedMenu.activeInHierarchy);
        }
    }

    public void PauseGame()
    {
        if (SceneManager.GetActiveScene().buildIndex >= 3)
        {
            if (!m_gameplayPausedMenu.activeInHierarchy)
            {
                OnGameplayPaused(true);
            }
        }
    }

    public void OnGameplayPaused(bool paused = false)
    {
        if (!paused)
        {
            Time.timeScale = 1f;
            m_isGamePlayPaused = false;
            m_gameplayPausedMenu.SetActive(false);
        }
        else
        {
            Time.timeScale = 0.00001f;
            m_isGamePlayPaused = true;
            m_gameplayPausedMenu.SetActive(true);
        }
    }

    public void RestartLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadLevel(int levelIndex = -1, float delay = 0f)
    {
        Time.timeScale = 1;

        if (m_isSceneLoading)
        {
            return;
        }
        else
        {
            m_isSceneLoading = true;
        }

        StartCoroutine(LoadYourAsyncScene(delay, levelIndex));
    }

    public void LoadLevel(string levelName)
    {
        if (!m_isSceneLoading)
        {
            m_isSceneLoading = true;

            Time.timeScale = 1f;

            m_levelToLoad = levelName;
            Debug.Log("New Level Load : " + m_levelToLoad);

            StartCoroutine(LoadYourAsyncScene());
        }
    }

    private IEnumerator LoadYourAsyncScene(float delayInSec = 0f, int levelIndex = -1)
    {
        yield return new WaitForSeconds(delayInSec);

        if (m_crossFadeAnimation == null)
        {
            m_crossFadeAnimation = new GameObject("Cross fade Anim");
        }
        else
        {
            m_crossFadeAnimation.SetActive(true);
        }

        AsyncOperation asyncLoad;
        if (levelIndex >= 0)
        {
            asyncLoad = SceneManager.LoadSceneAsync(levelIndex);
        }
        else
        {
            asyncLoad = SceneManager.LoadSceneAsync(m_levelToLoad);
        }
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9F)
        {
            yield return null;
        }

        Debug.Log(asyncLoad.progress);

        yield return new WaitForSeconds(1f);

        asyncLoad.allowSceneActivation = true;

        m_isSceneLoading = false;
    }

    public void QuitRequest()
    {
        Debug.Log("Quit Requested!");
        Application.Quit();
    }
}
