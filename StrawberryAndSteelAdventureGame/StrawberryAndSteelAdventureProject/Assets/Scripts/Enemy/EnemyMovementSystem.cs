
using System;
using UnityEngine;

public class EnemyMovementSystem
{
    private readonly EnemyController controller;
    private readonly EnemyPhysicsSettings physicsSettings;

    private IEnemyMovementBehaviour activeBehaviour;
    private IEnemyMovementAction activeAction;

    private JumpBehaviour jumpBehaviour;

    private bool isLocked;

    public bool HasActiveBehaviour => activeBehaviour != null;
    public bool HasActiveAction => activeAction != null;

    public bool IsLocked => isLocked;

    public bool IsMoving =>
        activeAction != null ||
        (activeBehaviour != null && activeBehaviour.IsMoving);

    public bool IsJumping =>
        jumpBehaviour != null && jumpBehaviour.IsActive;

    public IEnemyMovementBehaviour ActiveBehaviour => activeBehaviour;
    public IEnemyMovementAction ActiveAction => activeAction;

    public bool IsNormalMovementActive => activeBehaviour != null;
    public bool IsSpecialMovementActive => activeAction != null;

    public EnemyMovementSystem(EnemyController controller)
    {
        this.controller = controller;

        physicsSettings = new EnemyPhysicsSettings();
        physicsSettings.Initialize(controller.EnemyRigidbody);
    }

    // Normal Movement

    public void SetBehaviour(IEnemyMovementBehaviour behaviour)
    {
        if (isLocked || activeBehaviour == behaviour)
            return;

        StopActiveAction();
        activeBehaviour?.Stop();

        activeBehaviour = behaviour;

        if (activeBehaviour is IEnemyMovementPhysicsBehaviour physicsBehaviour)
        {
            physicsSettings.ApplyMovementPhysics(physicsBehaviour);
        }
        else
        {
            physicsSettings.RestoreOriginalSettings();
        }

        activeBehaviour?.Initialize(controller);
    }

    public void Move(Vector2 direction)
    {
        if (isLocked)
            return;

        activeBehaviour?.Move(direction);
    }

    public void StopNormalMovement()
    {
        activeBehaviour?.Stop();
    }

    public void SetSpeed(float speed)
    {
        if (isLocked)
            return;

        activeBehaviour?.SetSpeed(speed);
    }

    // Jump

    public void SetJumpBehaviour(JumpBehaviour behaviour)
    {
        if (isLocked)
            return;

        if (jumpBehaviour != null && jumpBehaviour != behaviour)
            jumpBehaviour.Stop();

        jumpBehaviour = behaviour;
    }

    public bool Jump()
    {
        if (isLocked || jumpBehaviour == null)
            return false;

        return TryActivateAction(
            jumpBehaviour,
            jumpBehaviour.StartJump);
    }

    // Special Movement Actions

    public bool TryActivateAction(
        IEnemyMovementAction action,
        Func<bool> startAction)
    {
        if (isLocked || action == null || startAction == null)
            return false;

        if (activeAction == action)
            return false;

        if (activeAction != null &&
            action.Priority <= activeAction.Priority)
        {
            return false;
        }

        if (!startAction())
            return false;

        if (!action.IsActive)
            return false;

        if (activeAction != null)
        {
            activeAction.Stop();
            activeAction = null;
        }

        activeAction = action;

        return true;
    }

    public void StopActiveAction()
    {
        if (activeAction == null)
            return;

        activeAction.Stop();
        activeAction = null;

        SynchronizeBehaviourVelocity();
    }

    public bool StopAction(IEnemyMovementAction action)
    {
        if (action == null || activeAction != action)
            return false;

        StopActiveAction();

        return true;
    }

    public bool IsActionActive(IEnemyMovementAction action)
    {
        return action != null && activeAction == action;
    }

    private void SynchronizeBehaviourVelocity()
    {
        if (activeBehaviour is not IEnemyMovementVelocityReceiver receiver)
            return;

        if (controller.EnemyRigidbody == null)
            return;

        receiver.SynchronizeVelocity(
            controller.EnemyRigidbody.linearVelocity);
    }

    // Movement Lock

    public void Lock()
    {
        if (isLocked)
            return;

        isLocked = true;

        StopActiveAction();
        StopNormalMovement();
    }

    public void Unlock()
    {
        if (!isLocked)
            return;

        isLocked = false;

        StopNormalMovement();
    }

    // Update


    public void Update(float deltaTime)
    {
        if (isLocked)
            return;

        if (activeAction != null)
        {
            activeAction.Update(deltaTime);

            if (!activeAction.IsActive)
            {
                activeAction = null;

                SynchronizeBehaviourVelocity();
            }
        }

        if (activeAction == jumpBehaviour)
        {
            activeBehaviour?.Update(deltaTime);
            return;
        }

        if (activeAction != null)
            return;

        activeBehaviour?.Update(deltaTime);
    }

    // Stop All

    public void Stop()
    {
        StopActiveAction();
        StopNormalMovement();
    }

    // Reset

    public void Reset()
    {
        StopActiveAction();

        jumpBehaviour?.Stop();
        jumpBehaviour = null;

        activeBehaviour?.Stop();
        activeBehaviour = null;

        activeAction = null;

        isLocked = false;

        physicsSettings.RestoreOriginalSettings();
    }
}
