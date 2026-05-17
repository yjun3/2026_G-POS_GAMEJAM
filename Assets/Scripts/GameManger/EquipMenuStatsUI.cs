using UnityEngine;
using TMPro;

public class EquipMenuStatsUI : MonoBehaviour
{
    [Header("Stat Texts")]
    public TMP_Text hpText;
    public TMP_Text gamePrefText;
    public TMP_Text luckText;
    public TMP_Text immunityText;
    public TMP_Text defenseText;

    CharacterFaceDraggable _character;
    ItemData _item;

    void OnEnable() => Refresh();

    public void SetCharacter(CharacterFaceDraggable character)
    {
        _character = character;
        Refresh();
    }

    public void SetItem(ItemData item)
    {
        _item = item;
        Refresh();
    }

    void Refresh()
    {
        if (_character == null) { ClearAll(); return; }
        if (ItemManager.Instance == null) { ClearAll(); return; }

        int avail = ItemManager.Instance.GetAvailableItems().Count;
        int total = ItemManager.Instance.TotalItemCount;
        int pref  = _character.gamePreference;

        int currentHP = CharacterManager.Instance != null
            ? CharacterManager.Instance.GetCurrentHP(_character)
            : _character.hp;
        int hpMod    = _item != null ? _item.hpMod     : 0;
        int luckMod  = _item != null ? _item.luckMod   : 0;
        int immMod   = _item != null ? _item.immunityMod : 0;
        int defMod   = _item != null ? _item.defenseMod  : 0;

        int maxHP     = _character.hp + hpMod;
        int eLuck     = EncounterManager.CalcEffective(_character.luck     + luckMod, pref, avail, total);
        int eImmunity = EncounterManager.CalcEffective(_character.immunity + immMod,  pref, avail, total);
        int eDefense  = EncounterManager.CalcEffective(_character.defense  + defMod,  pref, avail, total);

        // HP: 현재/최대 (아이템 보너스 별도 표시)
        Set(hpText,       "HP",   hpMod != 0
            ? $"{currentHP} / {maxHP}  ({_character.hp}+{hpMod})"
            : $"{currentHP} / {maxHP}");

        // Pref: 게임 선호도 (아이템 영향 없음)
        Set(gamePrefText, "Pref", $"{pref}");

        // 나머지: 최종값 (기본+아이템모드 → 게임선호도 적용)
        Set(luckText,     "Luck", CombinedLabel(_character.luck,     luckMod, eLuck));
        Set(immunityText, "Imm",  CombinedLabel(_character.immunity, immMod,  eImmunity));
        Set(defenseText,  "Def",  CombinedLabel(_character.defense,  defMod,  eDefense));
    }

    // 예시: "72  (60+20)" 또는 게임선호도 보정 시 "65  (60+20→×0.9)"
    static string CombinedLabel(int charBase, int itemMod, int effective)
    {
        int raw = charBase + itemMod;
        if (itemMod == 0)
            return raw == effective ? $"{effective}" : $"{effective}  ({charBase}→pref)";
        return raw == effective
            ? $"{effective}  ({charBase}+{itemMod})"
            : $"{effective}  ({charBase}+{itemMod}→pref)";
    }

    static void Set(TMP_Text t, string label, string value)
    {
        if (t != null) t.text = $"{label}: {value}";
    }

    void ClearAll()
    {
        Set(hpText,       "HP",   "-");
        Set(gamePrefText, "Pref", "-");
        Set(luckText,     "Luck", "-");
        Set(immunityText, "Imm",  "-");
        Set(defenseText,  "Def",  "-");
    }
}
