using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSelector : MonoBehaviour
{
    [Header("References")]
    public Button itemLogoButton;
    public GameObject itemListPanel;
    public Transform itemListContent;
    public GameObject itemButtonPrefab;
    public TMP_Text itemExplainText;
    public EquipMenuStatsUI statsUI;

    Image _logoImage;
    Sprite _defaultLogoSprite;

    ItemData _selected;
    public ItemData SelectedItem => _selected;

    readonly List<GameObject> _spawnedButtons = new();

    void Start()
    {
        _logoImage = itemLogoButton.GetComponent<Image>();
        _defaultLogoSprite = _logoImage.sprite;

        itemListPanel.SetActive(false);
        itemLogoButton.onClick.AddListener(OnItemLogoClick);
        if (itemExplainText != null) itemExplainText.text = "";
    }

    void OnItemLogoClick()
    {
        bool open = !itemListPanel.activeSelf;
        itemListPanel.SetActive(open);
        if (open) RefreshList();
    }

    void RefreshList()
    {
        foreach (var go in _spawnedButtons) Destroy(go);
        _spawnedButtons.Clear();

        // 맨 위에 "없음" 선택지 추가
        var noneGo = Instantiate(itemButtonPrefab, itemListContent);
        _spawnedButtons.Add(noneGo);
        noneGo.GetComponentInChildren<TMP_Text>().text = "None";
        noneGo.GetComponent<Button>().onClick.AddListener(SelectNone);

        var items = ItemManager.Instance.GetAvailableItems();
        foreach (var item in items)
        {
            var go = Instantiate(itemButtonPrefab, itemListContent);
            _spawnedButtons.Add(go);

            var btn = go.GetComponent<Button>();
            go.GetComponentInChildren<TMP_Text>().text = item.itemName;

            var captured = item;
            btn.onClick.AddListener(() => SelectItem(captured));
        }
    }

    void SelectNone()
    {
        itemListPanel.SetActive(false);
        ClearSelection();
        if (statsUI != null) statsUI.SetItem(null);
    }

    void SelectItem(ItemData item)
    {
        _selected = item;
        itemListPanel.SetActive(false);

        if (item.icon != null) _logoImage.sprite = item.icon;
        if (itemExplainText != null) itemExplainText.text = item.description;
        if (statsUI != null) statsUI.SetItem(item);
    }

    public void ClearSelection()
    {
        _selected = null;
        _logoImage.sprite = _defaultLogoSprite;
        if (itemExplainText != null) itemExplainText.text = "";
    }
}
