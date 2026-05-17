using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Image))]
public class ExpeditionFaceSlot : MonoBehaviour, IDropHandler
{
    public TMP_Text nameText;
    public TMP_Text propertyText;
    public EquipMenuStatsUI statsUI;

    Image _face;

    void Awake()
    {
        _face = GetComponent<Image>();
    }

    public CharacterFaceDraggable AssignedCharacter { get; private set; }

    public void OnDrop(PointerEventData e)
    {
        if (e.pointerDrag == null) return;
        if (!e.pointerDrag.TryGetComponent<CharacterFaceDraggable>(out var draggable)) return;
        if (CharacterManager.Instance != null && !CharacterManager.Instance.IsAlive(draggable)) return;
        Assign(draggable);
    }

    public void Assign(CharacterFaceDraggable draggable)
    {
        AssignedCharacter = draggable;
        _face.sprite = draggable.FaceSprite;
        _face.color = Color.white;
        nameText.text = draggable.characterName;
        propertyText.text = draggable.propertyDescription;
        if (statsUI != null) statsUI.SetCharacter(draggable);
    }

    public void Clear()
    {
        AssignedCharacter = null;
        _face.sprite = null;
        _face.color = Color.white;
        nameText.text = "";
        propertyText.text = "";
        if (statsUI != null) statsUI.SetCharacter(null);
    }
}
