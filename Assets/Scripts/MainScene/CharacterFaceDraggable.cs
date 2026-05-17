using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterFaceDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string characterName;
    [TextArea] public string propertyDescription;

    [Header("Base Stats")]
    [Range(0, 100)] public int hp = 80;
    [Range(0, 100)] public int gamePreference = 70;
    [Range(0, 100)] public int luck = 60;
    [Range(0, 100)] public int immunity = 60;
    [Range(0, 100)] public int defense = 60;

    Image _source;
    Canvas _rootCanvas;
    GameObject _ghost;

    void Awake()
    {
        _source = GetComponent<Image>();
        _rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
    }

    public Sprite FaceSprite => _source.sprite;

    public void OnBeginDrag(PointerEventData e)
    {
        _ghost = new GameObject("DragGhost");
        var img = _ghost.AddComponent<Image>();
        img.sprite = _source.sprite;
        img.raycastTarget = false;

        var rt = _ghost.GetComponent<RectTransform>();
        rt.SetParent(_rootCanvas.transform, false);
        rt.sizeDelta = _source.rectTransform.sizeDelta;
        rt.SetAsLastSibling();

        _source.color = new Color(1f, 1f, 1f, 0.4f);
        MoveGhost(e);
    }

    public void OnDrag(PointerEventData e) => MoveGhost(e);

    public void OnEndDrag(PointerEventData e)
    {
        if (_ghost != null) Destroy(_ghost);
        _source.color = Color.white;
    }

    void MoveGhost(PointerEventData e)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rootCanvas.transform as RectTransform, e.position, e.pressEventCamera, out var localPos);
        _ghost.GetComponent<RectTransform>().localPosition = localPos;
    }
}
