using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndingUI : MonoBehaviour
{
    public static EndingUI Instance { get; private set; }

    [Header("Ending Image")]
    public Image endingImage;

    [Header("Sprites")]
    public Sprite trueEndingSprite;
    public Sprite normalEndingSprite;
    public Sprite badEndingSprite;

    bool _showing;
    float _showTime;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!_showing) return;
        // 0.5초 후 클릭 감지 (의도치 않은 즉시 클릭 방지)
        if (Time.unscaledTime - _showTime < 0.5f) return;
        if (Input.GetMouseButtonDown(0))
            SceneManager.LoadScene("TitleScene");
    }

    public void ShowTrue()   => Show(trueEndingSprite);
    public void ShowNormal() => Show(normalEndingSprite);
    public void ShowBad()    => Show(badEndingSprite);

    void Show(Sprite sprite)
    {
        gameObject.SetActive(true);
        if (endingImage != null) endingImage.sprite = sprite;
        _showing  = true;
        _showTime = Time.unscaledTime;
    }
}
