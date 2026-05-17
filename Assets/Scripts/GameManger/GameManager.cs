using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Initializing, Playing, Paused }
    public GameState State { get; private set; } = GameState.Initializing;

    public int MaxDays = 10;
    public int CharacterCount { get; private set; } = 5;

    public bool CanInput => State != GameState.Initializing;

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
        if (DayNightManager.Instance != null) DayNightManager.Instance.ResetToStart();
        if (UIManager.Instance != null) UIManager.Instance.ResetUI();
        if (ItemManager.Instance != null) ItemManager.Instance.Reset();
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
            TriggerBadEnding();
    }

    void TriggerBadEnding()
    {
        Debug.Log("Bad Ending");
        // TODO: 배드엔딩 씬 전환
    }

    // MaxDays 버텼을 때 생존 인원에 따라 엔딩 분기
    public void TriggerEnding()
    {
        int alive = CharacterManager.Instance != null ? CharacterManager.Instance.AliveCount : 0;
        if (alive == 0)
            Debug.Log("Bad Ending");
        else if (alive < 5)
            Debug.Log("Normal Ending");
        else
            Debug.Log("True Ending");
        // TODO: 엔딩 씬 전환
    }
}
