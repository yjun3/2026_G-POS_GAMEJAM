using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    [SerializeField] List<CharacterFaceDraggable> characters = new();

    class RuntimeData
    {
        public int currentHP;
        public bool isAlive = true;
        public bool isInfected = false;
    }

    readonly Dictionary<CharacterFaceDraggable, RuntimeData> _runtime = new();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void ResetToStart()
    {
        _runtime.Clear();
        foreach (var c in characters)
            _runtime[c] = new RuntimeData { currentHP = c.hp };
    }

    public int GetCurrentHP(CharacterFaceDraggable c)
        => _runtime.TryGetValue(c, out var d) ? d.currentHP : c.hp;

    public bool IsAlive(CharacterFaceDraggable c)
        => !_runtime.TryGetValue(c, out var d) || d.isAlive;

    public bool IsInfected(CharacterFaceDraggable c)
        => _runtime.TryGetValue(c, out var d) && d.isInfected;

    public int AliveCount => characters.Count(c => IsAlive(c));

    public List<CharacterFaceDraggable> GetAliveCharacters()
        => characters.Where(c => IsAlive(c)).ToList();

    // 체력 데미지 적용 — 0 이하면 사망
    public void ApplyDamage(CharacterFaceDraggable c, int damage)
    {
        if (!_runtime.TryGetValue(c, out var d) || !d.isAlive) return;
        d.currentHP = Mathf.Max(0, d.currentHP - damage);
        if (d.currentHP == 0)
        {
            d.isAlive = false;
            c.OnDied();
            GameManager.Instance.OnCharacterDied();
        }
    }

    // 감염으로 즉사
    public void Kill(CharacterFaceDraggable c, bool infected = false)
    {
        if (!_runtime.TryGetValue(c, out var d)) return;
        d.currentHP = 0;
        d.isAlive = false;
        d.isInfected = infected;
        c.OnDied();
        GameManager.Instance.OnCharacterDied();
    }
}
