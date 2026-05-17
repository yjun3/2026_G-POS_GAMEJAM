using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    public static EncounterManager Instance { get; private set; }

    [Header("Encounter Settings")]
    public int zombieDamage = 30;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public enum Result { Safe, DamagedReturn, SurvivedBite, InfectedDead, HPDead }

    // 인카운터 흐름:
    // 1. 운 체크 → 통과 시 안전 귀환
    // 2. 방어력 체크 → 통과 시 물리지 않음 → 체력 데미지
    // 3. 면역력 체크 → 통과 시 감염 면역 → 귀환 / 실패 시 감염사
    public Result RunEncounter(CharacterFaceDraggable character, ItemData item)
    {
        int avail = ItemManager.Instance.GetAvailableItems().Count;
        int total = ItemManager.Instance.TotalItemCount;
        int pref   = character.gamePreference;

        int eLuck     = CalcEffective(character.luck     + (item != null ? item.luckMod     : 0), pref, avail, total);
        int eDefense  = CalcEffective(character.defense  + (item != null ? item.defenseMod  : 0), pref, avail, total);
        int eImmunity = CalcEffective(character.immunity + (item != null ? item.immunityMod : 0), pref, avail, total);

        Debug.Log($"[Encounter] {character.characterName} | 운:{eLuck} 방어:{eDefense} 면역:{eImmunity}");

        // 1. 운 — 공격 회피
        if (Random.Range(0, 100) < eLuck)
        {
            Debug.Log("결과: 안전 귀환");
            return Result.Safe;
        }

        // 2. 방어력 — 물림 방어
        if (Random.Range(0, 100) < eDefense)
        {
            // 물리적 타격만 (물리지 않음)
            CharacterManager.Instance.ApplyDamage(character, zombieDamage);
            if (!CharacterManager.Instance.IsAlive(character))
            {
                Debug.Log("결과: 체력 0 사망");
                return Result.HPDead;
            }
            Debug.Log("결과: 부상 후 귀환");
            return Result.DamagedReturn;
        }

        // 3. 물림 — 면역력으로 감염 여부 결정
        if (Random.Range(0, 100) < eImmunity)
        {
            Debug.Log("결과: 물렸지만 면역으로 귀환");
            return Result.SurvivedBite;
        }

        CharacterManager.Instance.Kill(character, infected: true);
        Debug.Log("결과: 감염 사망");
        return Result.InfectedDead;
    }

    // 게임 선호도 × 아이템 비율로 스탯 배율 계산 (체력 제외 모든 스탯)
    public static int CalcEffective(int stat, int gamePreference, int availItems, int totalItems)
    {
        if (totalItems == 0) return stat;
        float ratio = (float)availItems / totalItems;
        float prefF = gamePreference / 100f;
        // prefF 높고 ratio 낮을수록 큰 페널티, 높을수록 큰 보너스
        float mult = Mathf.Lerp(1f - prefF, 1f + prefF, ratio);
        return Mathf.Clamp(Mathf.RoundToInt(stat * mult), 0, 100);
    }
}
