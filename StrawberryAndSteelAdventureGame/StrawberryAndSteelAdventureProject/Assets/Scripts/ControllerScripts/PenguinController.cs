using UnityEngine;

public sealed class PenguinController
{
    private const float SlideWaveAmplitude = 0.25f;
    private const float SlideWaveCycles = 1f;
    private const float SlideRotationMultiplier = 2f;

    public enum Action
    {
        Run,
        Slide,
        JumpAtMidpoint
    }

    private readonly Transform penguinTransform;
    private readonly Animator animator;
    private readonly float startX;
    private readonly float endX;
    private readonly float laneY;
    private readonly float speed;
    private readonly Action action;
    private readonly float jumpDuration;
    private readonly float jumpHeight;

    private bool jumping;
    private bool hasJumped;
    private bool complete;
    private float jumpTime;

    public bool IsComplete => complete;

    public PenguinController(
        Transform penguinTransform,
        Animator animator,
        float startX,
        float endX,
        float laneY,
        float speed,
        Action action,
        float jumpDuration,
        float jumpHeight)
    {
        this.penguinTransform = penguinTransform;
        this.animator = animator;
        this.startX = startX;
        this.endX = endX;
        this.laneY = laneY;
        this.speed = Mathf.Max(0.01f, speed);
        this.action = action;
        this.jumpDuration = Mathf.Max(0.1f, jumpDuration);
        this.jumpHeight = Mathf.Max(0f, jumpHeight);

        penguinTransform.localPosition = new Vector3(startX, laneY, penguinTransform.localPosition.z);
        Play(action == Action.Slide ? "PenguinSlide" : "PenguinRun");
    }

    public void Tick(float deltaTime)
    {
        if (complete)
            return;

        float nextX = Mathf.MoveTowards(
            penguinTransform.localPosition.x,
            endX,
            speed * deltaTime);

        if (action == Action.JumpAtMidpoint && !hasJumped &&
            Mathf.Abs(nextX - startX) >= Mathf.Abs(endX - startX) * 0.5f)
        {
            jumping = true;
            hasJumped = true;
            jumpTime = 0f;
            Play("PenguinJump");
        }

        float y = laneY;
        if (action == Action.Slide)
        {
            float signedDistance = endX - startX;
            float distance = Mathf.Abs(signedDistance);
            float progress = distance <= Mathf.Epsilon
                ? 1f
                : Mathf.Clamp01(Mathf.Abs(nextX - startX) / distance);
            float wavePhase = progress * Mathf.PI * 2f * SlideWaveCycles;
            y += Mathf.Sin(wavePhase) * SlideWaveAmplitude;

            float slope = SlideWaveAmplitude * Mathf.PI * 2f * SlideWaveCycles *
                Mathf.Cos(wavePhase) / signedDistance;
            float rotationZ = Mathf.Atan(slope) * Mathf.Rad2Deg * SlideRotationMultiplier;
            penguinTransform.localRotation = Quaternion.Euler(0f, 0f, rotationZ);
        }

        if (jumping)
        {
            jumpTime += deltaTime;
            float progress = Mathf.Clamp01(jumpTime / jumpDuration);
            y += Mathf.Sin(progress * Mathf.PI) * jumpHeight;
            if (progress >= 1f)
            {
                jumping = false;
                Play("PenguinRun");
            }
        }

        penguinTransform.localPosition = new Vector3(nextX, y, penguinTransform.localPosition.z);

        if (Mathf.Abs(nextX - endX) <= 0.001f)
        {
            penguinTransform.localPosition = new Vector3(endX, laneY, penguinTransform.localPosition.z);
            penguinTransform.localRotation = Quaternion.identity;
            complete = true;
        }
    }

    private void Play(string stateName)
    {
        if (animator != null)
        {
            animator.SetBool("IsIdle", stateName == "PenguinIdle");
            animator.SetBool("IsRun", stateName == "PenguinRun");
            animator.SetBool("IsSlide", stateName == "PenguinSlide");
            animator.SetBool("IsJump", stateName == "PenguinJump");
            animator.Play(stateName, 0, 0f);
        }
    }
}
