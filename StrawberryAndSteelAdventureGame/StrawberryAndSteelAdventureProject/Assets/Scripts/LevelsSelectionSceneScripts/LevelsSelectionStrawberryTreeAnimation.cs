
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(StrawberryTreeAnimationController))]
public class LevelsSelectionStrawberryTreeAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float idleDuration = 1.5f;
    [SerializeField] private string actionStateName = "StrawberrySittingOnATreeAction";

    private Animator animator;
    private StrawberryTreeAnimationController animationController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animationController = GetComponent<StrawberryTreeAnimationController>();
    }

    private void Start()
    {
        StartCoroutine(PlayAnimationLoop());
    }

    private IEnumerator PlayAnimationLoop()
    {
        while (true)
        {
            animationController.PlayIdle();

            yield return new WaitForSeconds(idleDuration);

            animationController.PlayAction();

            // Action state'ine geçilmesini bekle.
            while (animator.IsInTransition(0) ||
                   !animator.GetCurrentAnimatorStateInfo(0).IsName(actionStateName))
            {
                yield return null;
            }

            // Action animasyonunun tamamlanmasýný bekle.
            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
            }
        }
    }
}
