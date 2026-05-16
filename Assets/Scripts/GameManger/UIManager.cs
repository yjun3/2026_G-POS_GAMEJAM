using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    bool isPaused = false;
    bool isEquipOpen = false;
    public GameObject pauseMenu;
    public GameObject equipMenu;
    public GameObject blocker;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    void TogglePause()
    {
        if (isEquipOpen)
        {
            OnEquipCancelButtonClick();
            return;
        }

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        pauseMenu.SetActive(isPaused);
        blocker.SetActive(isPaused);
    }

    public void OnResumeButtonClick()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        blocker.SetActive(false);
    }

    public void OnGoToTitleButtonClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }

    public void OnEquipButtonClick()
    {
        isEquipOpen = !isEquipOpen;
        Time.timeScale = isEquipOpen ? 0f : 1f;
        equipMenu.SetActive(isEquipOpen);
        blocker.SetActive(isEquipOpen);
    }

    public void OnEquipCancelButtonClick()
    {
        isEquipOpen = false;
        Time.timeScale = 1f;
        equipMenu.SetActive(false);
        blocker.SetActive(false);
    }
}
