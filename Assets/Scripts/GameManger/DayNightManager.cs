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
    public float fadeDuration = 2f;
    public float nightBlackHoldDuration = 2f;

    [Header("References")]
    public ExpeditionFaceSlot faceSlot;
    public ItemSelector itemSelector;

    public bool IsDay { get; private set; } = true;
    public int CurrentDay { get; private set; } = 1;
    public bool IsTransitioning { get; private set; } = false;

    EncounterResultData _lastResult;
    CharacterFaceDraggable _expeditionCharacter;
    ItemData _expeditionItem;
    static readonly WaitForSecondsRealtime ResultDelay = new(0.3f);

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
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGo();
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
            // 낮→밤: 출발 정보 저장 후 슬롯 초기화
            _expeditionCharacter = faceSlot != null ? faceSlot.AssignedCharacter : null;
            _expeditionItem      = itemSelector != null ? itemSelector.SelectedItem : null;
            ClearExpedition();
        }
        else
        {
            // 밤→낮: 귀환 → 인카운터 실행
            RunReturnEncounter();
            CurrentDay++;

            // 암전 중 효과음 + 홀드
            if (AudioManager.Instance != null) AudioManager.Instance.PlayHit();
            yield return new WaitForSecondsRealtime(nightBlackHoldDuration);
        }

        UpdateUI();

        yield return StartCoroutine(FadeRoutine(1f, 0f));

        // 결과창 먼저 표시
        if (IsDay && _lastResult != null && ResultMenuUI.Instance != null)
        {
            yield return ResultDelay;
            ResultMenuUI.Instance.Show(_lastResult);
            _lastResult = null;
            yield return new WaitUntil(() => !ResultMenuUI.Instance.IsShowing);
        }

        IsTransitioning = false;

        // 결과창 닫힌 후 엔딩 체크
        if (!IsDay) yield break;

        if (GameManager.Instance.ConsumeBadEndingPending())
        {
            GameManager.Instance.TriggerBadEnding();
            yield break;
        }

        if (CurrentDay > GameManager.Instance.MaxDays)
        {
            GameManager.Instance.TriggerEnding();
        }
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

    void RunReturnEncounter()
    {
        if (_expeditionCharacter == null)
        {
            Debug.LogWarning("[DayNight] 귀환 캐릭터 없음 — 출발 시 편성이 비어있었음");
            return;
        }
        if (EncounterManager.Instance == null)
        {
            Debug.LogError("[DayNight] EncounterManager.Instance is null");
            return;
        }

        _lastResult = EncounterManager.Instance.RunEncounter(
            _expeditionCharacter,
            _expeditionItem
        );
        _expeditionCharacter = null;
        _expeditionItem      = null;
    }

    void ClearExpedition()
    {
        if (faceSlot != null) faceSlot.Clear();
        if (itemSelector != null) itemSelector.ClearSelection();
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
