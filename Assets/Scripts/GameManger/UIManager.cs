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

    public void ResetUI()
    {
        isPaused = false;
        isEquipOpen = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        equipMenu.SetActive(false);
        blocker.SetActive(false);
    }

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.CanInput) return;
        if (DayNightManager.Instance != null && DayNightManager.Instance.IsTransitioning) return;
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
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButton();
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        blocker.SetActive(false);
    }

    public void OnGoToTitleButtonClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButton();
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }

    public void OnEquipButtonClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayEquip();
        isEquipOpen = !isEquipOpen;
        Time.timeScale = isEquipOpen ? 0f : 1f;
        equipMenu.SetActive(isEquipOpen);
        blocker.SetActive(isEquipOpen);
    }

    public void OnEquipCancelButtonClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButton();
        isEquipOpen = false;
        Time.timeScale = 1f;
        equipMenu.SetActive(false);
        blocker.SetActive(false);
    }
}
