using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultMenuUI : MonoBehaviour
{
    public static ResultMenuUI Instance { get; private set; }

    [Header("Face")]
    public Image expeditionFace;

    [Header("Character Status Icon (둘 중 하나 표시)")]
    public Image survivalImage;   // 생존
    public Image infectionImage;  // 감염

    [Header("Item Status Icon (둘 중 하나 표시)")]
    public Image itemOkImage;     // 멀쩡
    public Image itemBrokenImage; // 부서짐

    [Header("HP")]
    public TMP_Text hpChangeText;

    [Header("Continue")]
    public Button continueButton;

    public bool IsShowing { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        gameObject.SetActive(false);
    }

    void Start()
    {
        continueButton.onClick.AddListener(OnContinue);
    }

    public void Show(EncounterResultData result)
    {
        gameObject.SetActive(true);
        IsShowing = true;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayReturn();

        expeditionFace.sprite = result.character.FaceSprite;

        // HP 변화 (0 미만 방지)
        int hpAfter = Mathf.Max(0, result.hpAfter);
        int delta = hpAfter - result.hpBefore;
        hpChangeText.text = delta == 0 ? "HP ±0" : $"HP {delta:+#;-#;0}";
        hpChangeText.color = delta < 0 ? new Color(0.8f, 0.1f, 0.1f) : Color.black;

        // 캐릭터 상태: 생존/감염
        if (survivalImage != null)  survivalImage.gameObject.SetActive(!result.isInfected);
        if (infectionImage != null) infectionImage.gameObject.SetActive(result.isInfected);

        // 아이템 상태: 멀쩡/부서짐
        bool hasItem = result.item != null;
        if (itemOkImage != null)     itemOkImage.gameObject.SetActive(hasItem && !result.itemDestroyed);
        if (itemBrokenImage != null) itemBrokenImage.gameObject.SetActive(hasItem && result.itemDestroyed);
    }

    void OnContinue()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButton();
        IsShowing = false;
        gameObject.SetActive(false);
    }
}
