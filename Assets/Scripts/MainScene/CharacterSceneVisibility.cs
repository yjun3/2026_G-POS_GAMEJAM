using UnityEngine;

// 씬의 걷는 캐릭터 오브젝트에 붙이기
// character 필드에 해당 CharacterFaceDraggable 연결
public class CharacterSceneVisibility : MonoBehaviour
{
    public CharacterFaceDraggable character;

    bool _dead;

    void Update()
    {
        if (_dead) return;
        if (CharacterManager.Instance == null || character == null) return;
        if (!CharacterManager.Instance.IsAlive(character))
        {
            _dead = true;
            gameObject.SetActive(false);
        }
    }
}
