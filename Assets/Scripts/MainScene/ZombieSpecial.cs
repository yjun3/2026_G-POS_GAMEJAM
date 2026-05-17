using System.Collections.Generic;
using UnityEngine;

// Zombie_02 오브젝트에 붙이기
public class ZombieSpecial : MonoBehaviour
{
    [System.Serializable]
    public struct CharacterZombieMapping
    {
        public CharacterFaceDraggable character;
        public string animStateName;
    }

    [Tooltip("캐릭터와 좀비 애니메이션 상태 이름 매핑 (5명 모두 등록)")]
    public List<CharacterZombieMapping> mappings;

    Animator _anim;
    SpriteRenderer _sr;
    bool _lastIsDay;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _sr   = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        _lastIsDay = DayNightManager.Instance == null || DayNightManager.Instance.IsDay;
        Apply(_lastIsDay);
    }

    void Update()
    {
        if (DayNightManager.Instance == null) return;
        bool isDay = DayNightManager.Instance.IsDay;
        if (isDay == _lastIsDay) return;
        _lastIsDay = isDay;
        Apply(isDay);
    }

    void Apply(bool isDay)
    {
        if (isDay)
        {
            if (_sr != null) _sr.enabled = false;
            return;
        }

        // 죽은 캐릭터 목록 수집
        var dead = new List<CharacterZombieMapping>();
        foreach (var m in mappings)
        {
            if (m.character == null) continue;
            if (CharacterManager.Instance == null) continue;
            if (!CharacterManager.Instance.IsAlive(m.character))
                dead.Add(m);
        }

        if (dead.Count == 0)
        {
            if (_sr != null) _sr.enabled = false;
            return;
        }

        if (_sr != null) _sr.enabled = true;
        var chosen = dead[Random.Range(0, dead.Count)];
        if (_anim != null)
            _anim.Play(chosen.animStateName, -1, Random.value);
    }
}
