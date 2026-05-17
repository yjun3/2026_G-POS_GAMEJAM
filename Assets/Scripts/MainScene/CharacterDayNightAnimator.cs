using UnityEngine;

public class CharacterDayNightAnimator : MonoBehaviour
{
    Animator _anim;
    bool _lastIsDay;

    void Awake()
    {
        _anim = GetComponent<Animator>();
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
        if (_anim == null) return;
        if (isDay)
        {
            _anim.speed = 1f;
        }
        else
        {
            _anim.Play(0, -1, 0f); // 첫 프레임(서있는 자세)으로 리셋
            _anim.speed = 0f;
        }
    }
}
