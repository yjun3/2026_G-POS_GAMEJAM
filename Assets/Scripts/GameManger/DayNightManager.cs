using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayNightManager : MonoBehaviour
{
    public static DayNightManager Instance { get; private set; }

    [Header("UI")]
    public Image dayImage;
    public Sprite sunSprite;
    public Sprite moonSprite;
    public TextMeshProUGUI dayText;

    public bool IsDay { get; private set; } = true;
    public int CurrentDay { get; private set; } = 1;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    public void OnGoButtonPressed()
    {
        IsDay = !IsDay;

        if (IsDay)
            CurrentDay++;

        UpdateUI();
    }

    void UpdateUI()
    {
        dayImage.sprite = IsDay ? sunSprite : moonSprite;
        dayText.text = $"Day {CurrentDay}";
    }
}
