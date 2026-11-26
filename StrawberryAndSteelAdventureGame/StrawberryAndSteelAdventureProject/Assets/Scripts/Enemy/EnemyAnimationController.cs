
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

    /// <summary>Returns the live Animator state and clip names for diagnostics.</summary>
    public string GetDebugStateDescription()
    {
        if (animator == null || !animator.isActiveAndEnabled ||
            animator.runtimeAnimatorController == null || animator.layerCount == 0)
        {
            return "Animator unavailable";
        }

        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        string currentStateName = GetStateName(currentState);
        string currentClips = GetClipNames(animator.GetCurrentAnimatorClipInfo(0));

        if (!animator.IsInTransition(0))
            return $"{currentStateName} | clip: {currentClips}";

        AnimatorStateInfo nextState = animator.GetNextAnimatorStateInfo(0);
        string nextStateName = GetStateName(nextState);
        string nextClips = GetClipNames(animator.GetNextAnimatorClipInfo(0));
        return $"Transition: {currentStateName} [{currentClips}] -> {nextStateName} [{nextClips}]";
    }

    private static string GetStateName(AnimatorStateInfo state)
    {
        if (state.IsName("SpiderIdle")) return "SpiderIdle";
        if (state.IsName("SpiderWalk")) return "SpiderWalk";
        if (state.IsName("SpiderAttack")) return "SpiderAttack";
        if (state.IsName("SpiderHurt")) return "SpiderHurt";
        if (state.IsName("SpiderDeath")) return "SpiderDeath";

        return $"State#{state.shortNameHash}";
    }

    private static string GetClipNames(AnimatorClipInfo[] clips)
    {
        if (clips == null || clips.Length == 0)
            return "none";

        string[] names = new string[clips.Length];
        for (int i = 0; i < clips.Length; i++)
            names[i] = clips[i].clip != null ? clips[i].clip.name : "unknown";

        return string.Join(", ", names);
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
