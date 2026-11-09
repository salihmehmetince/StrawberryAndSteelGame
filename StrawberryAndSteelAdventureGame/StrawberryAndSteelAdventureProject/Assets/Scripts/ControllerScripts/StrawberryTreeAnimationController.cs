
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class StrawberryTreeAnimationController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        PlayIdle();
    }

    public void PlayIdle()
    {
        animator.SetBool("IsAction", false);
        animator.SetBool("IsIdle", true);
    }

    public void PlayAction()
    {
        animator.SetBool("IsIdle", false);
        animator.SetBool("IsAction", true);
    }
}
