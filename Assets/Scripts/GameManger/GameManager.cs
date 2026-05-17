using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Initializing, Playing, Paused, Ending }
    public GameState State { get; private set; } = GameState.Initializing;

    public int MaxDays = 10;
    public int CharacterCount { get; private set; } = 5;

    public bool CanInput => State == GameState.Playing;

    bool _badEndingPending;
    public bool ConsumeBadEndingPending()
    {
        bool v = _badEndingPending;
        _badEndingPending = false;
        return v;
    }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        CharacterCount = 5;
        _badEndingPending = false;
        if (DayNightManager.Instance != null) DayNightManager.Instance.ResetToStart();
        if (UIManager.Instance != null)       UIManager.Instance.ResetUI();
        if (ItemManager.Instance != null)     ItemManager.Instance.Reset();
        if (CharacterManager.Instance != null) CharacterManager.Instance.ResetToStart();
        State = GameState.Playing;
    }

    public void SetPaused(bool paused)
    {
        if (!CanInput) return;
        State = paused ? GameState.Paused : GameState.Playing;
    }

    public void OnCharacterDied()
    {
        CharacterCount--;
        if (CharacterCount <= 0)
            _badEndingPending = true;  // 인카운터 후 결과창 닫힌 뒤에 처리
    }

    // 날짜 종료 엔딩 (생존자 수 기반)
    public void TriggerEnding()
    {
        State = GameState.Ending;
        if (EndingUI.Instance == null) return;

        int alive = CharacterManager.Instance != null ? CharacterManager.Instance.AliveCount : 0;
        if (alive >= 5)     EndingUI.Instance.ShowTrue();
        else if (alive >= 1) EndingUI.Instance.ShowNormal();
        else                 EndingUI.Instance.ShowBad();
    }

    // 전원 사망 엔딩
    public void TriggerBadEnding()
    {
        State = GameState.Ending;
        if (EndingUI.Instance != null)
            EndingUI.Instance.ShowBad();
    }
}
