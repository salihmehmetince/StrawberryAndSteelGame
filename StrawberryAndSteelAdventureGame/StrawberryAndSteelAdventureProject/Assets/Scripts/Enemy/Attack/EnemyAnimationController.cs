
using UnityEngine;

public class EnemyAnimationController
{
    private Animator animator;

    private int currentAnimationState = -1;

    private static readonly int IsIdleHash = Animator.StringToHash("IsIdle");
    private static readonly int IsAttackHash = Animator.StringToHash("IsAttack");
    private static readonly int IsRunHash = Animator.StringToHash("IsRun");
    private static readonly int IsHurtHash = Animator.StringToHash("IsHurt");
    private static readonly int IsDeathHash = Animator.StringToHash("IsDeath");

    public bool IsInitialized => animator != null;

    public void Initialize(Animator animator)
    {
        Clear();

        if (animator == null)
        {
            Debug.LogError("EnemyAnimationController requires an Animator.");
            return;
        }

        this.animator = animator;

        ResetAnimationStates();
    }

    public void SetIdle()
    {
        SetAnimationState(IsIdleHash);
    }

    public void SetAttack()
    {
        SetAnimationState(IsAttackHash);
    }

    public void SetRun()
    {
        SetAnimationState(IsRunHash);
    }

    public void SetHurt()
    {
        SetAnimationState(IsHurtHash);
    }

    public void SetDeath()
    {
        SetAnimationState(IsDeathHash);
    }

    public void ResetAnimationStates()
    {
        if (!IsInitialized)
            return;

        animator.SetBool(IsIdleHash, false);
        animator.SetBool(IsAttackHash, false);
        animator.SetBool(IsRunHash, false);
        animator.SetBool(IsHurtHash, false);
        animator.SetBool(IsDeathHash, false);

        currentAnimationState = -1;
    }

    private void SetAnimationState(int activeParameter)
    {
        if (!IsInitialized)
            return;

        if (currentAnimationState == activeParameter)
            return;

        ResetAnimationStates();

        animator.SetBool(activeParameter, true);
        currentAnimationState = activeParameter;
    }

    public void Clear()
    {
        animator = null;
        currentAnimationState = -1;
    }
}
