using UnityEngine;

public class MonsterCombatant : MonoBehaviour
{
    [Header("Animation State Names")]
    public string idleState   = "idle";
    public string walkState   = "walk";
    public string attackState = "attack";
    public string dieState    = "die";

    [Header("Movement")]
    public float moveSpeed = 0.4f;

    private Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void PlayIdle()   => animator.CrossFade(idleState,   0.2f);
    public void PlayWalk()   => animator.CrossFade(walkState,   0.2f);
    public void PlayAttack() => animator.CrossFade(attackState, 0.2f);
    public void PlayDie()    => animator.CrossFade(dieState,    0.2f);

    public void MoveTo(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0f;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    public void LookAt(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(direction);
    }
}
