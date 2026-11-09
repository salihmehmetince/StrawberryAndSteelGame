
using UnityEngine;

public class LoadingSceneController : MonoBehaviour
{
    [SerializeField] private Animator logoAnimator;

    private static readonly int IsIdle =
        Animator.StringToHash("IsIdle");

    private static readonly int IsAction =
        Animator.StringToHash("IsAction");

    public bool IsActionFinished { get; private set; }

    private bool isActionStarted;

    private void Start()
    {
        PlayIdleAnimation();
    }

    private void Update()
    {
        if (!isActionStarted)
            return;

        AnimatorStateInfo stateInfo =
            logoAnimator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("GameLogoAction") &&
            stateInfo.normalizedTime >= 1f)
        {
            IsActionFinished = true;
        }
    }

    public void PlayIdleAnimation()
    {
        IsActionFinished = false;
        isActionStarted = false;

        logoAnimator.SetBool(IsIdle, true);
        logoAnimator.SetBool(IsAction, false);
    }

    public void PlayExitAnimation()
    {
        IsActionFinished = false;
        isActionStarted = true;

        logoAnimator.SetBool(IsIdle, false);
        logoAnimator.SetBool(IsAction, true);
    }
}
