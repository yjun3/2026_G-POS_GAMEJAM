using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    public static EncounterManager Instance { get; private set; }

    [Header("Encounter Settings")]
    public int zombieDamage = 40;
    [Range(0, 100)] public int itemBreakChanceOnDamage = 80;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // 인카운터 흐름:
    // 1. 운 체크 → 통과 시 안전 귀환
    // 2. 방어력 체크 → 통과 시 물리 타격 → 체력 데미지
    // 3. 면역력 체크 → 실패 시 감염 (= 사망)
    public EncounterResultData RunEncounter(CharacterFaceDraggable character, ItemData item)
    {
        int avail = ItemManager.Instance.GetAvailableItems().Count;
        int total = ItemManager.Instance.TotalItemCount;
        int pref  = character.gamePreference;

        int eLuck     = CalcEffective(character.luck     + (item != null ? item.luckMod     : 0), pref, avail, total);
        int eDefense  = CalcEffective(character.defense  + (item != null ? item.defenseMod  : 0), pref, avail, total);
        int eImmunity = CalcEffective(character.immunity + (item != null ? item.immunityMod : 0), pref, avail, total);

        int hpBefore = CharacterManager.Instance.GetCurrentHP(character);
        bool infected = false;

        int rollLuck     = Random.Range(30, 100);
        int rollDefense  = Random.Range(40, 100);
        int rollImmunity = Random.Range(50, 100);

        bool attacked = rollLuck >= eLuck;
        bool bitten   = attacked && rollDefense >= eDefense;
        bool infectedByBite = bitten && rollImmunity >= eImmunity;


        if (attacked)
        {
            if (!bitten)                             // 방어 성공 → 물리 타격
            {
                CharacterManager.Instance.ApplyDamage(character, zombieDamage);

                if (!CharacterManager.Instance.IsAlive(character))
                    infected = true;
            }
            else if (infectedByBite)                 // 물림 + 면역 실패 → 감염
            {
                infected = true;
                CharacterManager.Instance.Kill(character, infected: true);
            }
        }

        int hpAfter = CharacterManager.Instance.GetCurrentHP(character);

        // 장비 파괴 여부 결정
        bool itemDestroyed = false;
        if (item != null && item.isBreakable)
        {
            if (infected)
                itemDestroyed = true;                // 감염 → 항상 파괴
            else if (Random.Range(0, 100) < itemBreakChanceOnDamage)
                itemDestroyed = true;                // 부상 → 확률 파괴
        }

        if (itemDestroyed)
            ItemManager.Instance.BreakItem(item);

        Debug.Log($"[결과] 감염:{infected} HP:{hpBefore}→{hpAfter} 장비파괴:{itemDestroyed}");

        return new EncounterResultData
        {
            character    = character,
            item         = item,
            hpBefore     = hpBefore,
            hpAfter      = hpAfter,
            isInfected   = infected,
            itemDestroyed = itemDestroyed
        };
    }

    public static int CalcEffective(int stat, int gamePreference, int availItems, int totalItems)
    {
        if (totalItems == 0) return stat;
        float ratio = (float)availItems / totalItems;
        float prefF = gamePreference / 100f * 0.4f;  // 민감도 완화
        float mult  = Mathf.Lerp(1f - prefF, 1f + prefF, ratio);
        return Mathf.Clamp(Mathf.RoundToInt(stat * mult), 0, 95);  // 95 캡: 항상 최소 5% 위험
    }
}
