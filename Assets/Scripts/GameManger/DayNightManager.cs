using UnityEngine;
using UnityEngine.UI;

public class DayNightManager : MonoBehaviour
{
    public static DayNightManager Instance { get; private set; }

    [Header("UI")]
    public Image dayImage;
    public Sprite sunSprite;
    public Sprite moonSprite;

    public bool IsDay { get; private set; } = true;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void OnGoButtonPressed()
    {
        IsDay = !IsDay;
        dayImage.sprite = IsDay ? sunSprite : moonSprite;
    }
}
