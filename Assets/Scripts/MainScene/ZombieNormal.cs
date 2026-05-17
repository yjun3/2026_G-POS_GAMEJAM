using UnityEngine;

// Zombie_01 오브젝트에 붙이기
public class ZombieNormal : MonoBehaviour
{
    static readonly string[] States = { "zombie_01", "zombie_02", "zombie_03" };

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
        if (_sr != null) _sr.enabled = !isDay;

        if (!isDay && _anim != null)
        {
            string state = States[Random.Range(0, States.Length)];
            _anim.Play(state, -1, Random.value);
        }
    }
}
