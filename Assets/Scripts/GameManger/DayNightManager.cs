using System.Collections;
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

    [Header("Buttons")]
    public Button goButton;
    public GameObject equipButton;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeDuration = 0.5f;

    [Header("References")]
    public ExpeditionFaceSlot faceSlot;
    public ItemSelector itemSelector;

    public bool IsDay { get; private set; } = true;
    public int CurrentDay { get; private set; } = 1;
    public bool IsTransitioning { get; private set; } = false;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (fadeImage != null) fadeImage.color = new Color(0, 0, 0, 0);
        UpdateUI();
    }

    void Update()
    {
        // 낮일 때만 캐릭터 배정 여부로 Go 버튼 활성 여부 갱신
        if (IsDay && !IsTransitioning && goButton != null)
            goButton.interactable = faceSlot != null && faceSlot.AssignedCharacter != null;
    }

    public void ResetToStart()
    {
        IsDay = true;
        CurrentDay = 1;
        UpdateUI();
    }

    public void OnGoButtonPressed()
    {
        if (IsTransitioning) return;
        if (IsDay && (faceSlot == null || faceSlot.AssignedCharacter == null)) return;
        StartCoroutine(TransitionRoutine());
    }

    IEnumerator TransitionRoutine()
    {
        IsTransitioning = true;
        if (goButton != null) goButton.interactable = false;

        // 낮→밤 전환 시 장비 메뉴 열려 있으면 닫기
        if (!IsDay == false && UIManager.Instance != null)
            UIManager.Instance.OnEquipCancelButtonClick();

        yield return StartCoroutine(FadeRoutine(0f, 1f));

        IsDay = !IsDay;

        if (!IsDay)
        {
            RunNightEncounter();
        }
        else
        {
            CurrentDay++;
            if (CurrentDay > GameManager.Instance.MaxDays)
            {
                GameManager.Instance.TriggerEnding();
                yield break;
            }
        }

        UpdateUI();

        yield return StartCoroutine(FadeRoutine(1f, 0f));

        IsTransitioning = false;
    }

    IEnumerator FadeRoutine(float from, float to)
    {
        if (fadeImage == null) yield break;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Lerp(from, to, elapsed / fadeDuration));
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, to);
    }

    void RunNightEncounter()
    {
        if (faceSlot == null || faceSlot.AssignedCharacter == null) return;

        var result = EncounterManager.Instance.RunEncounter(
            faceSlot.AssignedCharacter,
            itemSelector != null ? itemSelector.SelectedItem : null
        );

        Debug.Log($"[인카운터 결과] {result}");
    }

    void UpdateUI()
    {
        dayImage.sprite = IsDay ? sunSprite : moonSprite;
        dayText.text = $"Day {CurrentDay}";

        // 밤엔 장비 버튼 숨기기
        if (equipButton != null) equipButton.SetActive(IsDay);

        // 밤엔 Go 버튼 항상 활성화
        if (goButton != null && !IsDay) goButton.interactable = true;
    }
}
