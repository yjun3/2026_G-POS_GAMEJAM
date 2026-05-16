using UnityEngine;

public class RandomAnimationOffset : MonoBehaviour
{
    void Start()
    {
        Animator anim = GetComponent<Animator>();
        anim.Play(0, -1, Random.value);
    }
}
