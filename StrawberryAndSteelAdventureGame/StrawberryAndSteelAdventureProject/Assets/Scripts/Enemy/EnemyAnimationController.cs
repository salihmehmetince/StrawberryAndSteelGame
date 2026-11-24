
using UnityEngine;

public class EnemyAnimationController
{
    private readonly Animator animator;

    public EnemyAnimationController(Animator animator)
    {
        this.animator = animator;
    }

    public void PlayIdle()
    {
        SetAnimation("IsIdle");
    }

    public void PlayRun()
    {
        SetAnimation("IsRun");
    }

    public void PlayAttack()
    {
        SetAnimation("IsAttack");
    }

    public void PlayHurt()
    {
        SetAnimation("IsHurt");
    }

    /// <summary>True once the named one-shot state has completed its first play.</summary>
    public bool IsStateFinished(string stateName)
    {
        if (animator == null || !animator.isActiveAndEnabled ||
            animator.runtimeAnimatorController == null || animator.layerCount == 0)
        {
            return true;
        }

        // The requested state's normalized time has not started during entry blending.
        if (animator.IsInTransition(0) &&
            animator.GetNextAnimatorStateInfo(0).IsName(stateName))
        {
            return false;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(stateName) && stateInfo.normalizedTime >= 1f;
    }

    public void PlayDeath()
    {
        SetAnimation("IsDeath");
    }

    private void SetAnimation(string parameter)
    {
        animator.SetBool("IsIdle", parameter == "IsIdle");
        animator.SetBool("IsRun", parameter == "IsRun");
        animator.SetBool("IsAttack", parameter == "IsAttack");
        animator.SetBool("IsHurt", parameter == "IsHurt");
        animator.SetBool("IsDeath", parameter == "IsDeath");
    }
}
