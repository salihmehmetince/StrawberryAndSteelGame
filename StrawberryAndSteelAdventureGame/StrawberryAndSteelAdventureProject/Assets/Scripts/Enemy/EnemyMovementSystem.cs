
using System;
using UnityEngine;

public class EnemyMovementSystem
{
    private readonly EnemyController controller;
    private readonly EnemyPhysicsSettings physicsSettings;

    private IEnemyMovementBehaviour activeBehaviour;
    private IEnemyMovementAction activeAction;

    private JumpBehaviour jumpBehaviour;

    public bool HasActiveBehaviour => activeBehaviour != null;
    public bool HasActiveAction => activeAction != null;

    public bool IsMoving =>
        activeAction != null ||
        (activeBehaviour != null && activeBehaviour.IsMoving);

    public bool IsJumping =>
        jumpBehaviour != null && jumpBehaviour.IsActive;

    public EnemyMovementSystem(EnemyController controller)
    {
        this.controller = controller;

        physicsSettings = new EnemyPhysicsSettings();
        physicsSettings.Initialize(controller.EnemyRigidbody);
    }

    // Normal Movement

    public void SetBehaviour(IEnemyMovementBehaviour behaviour)
    {
        if (activeBehaviour == behaviour)
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

        if (activeBehaviour != null)
            activeBehaviour.Initialize(controller);
    }

    public void Move(Vector2 direction)
    {
        activeBehaviour?.Move(direction);
    }

    public void StopNormalMovement()
    {
        activeBehaviour?.Stop();
    }

    public void SetSpeed(float speed)
    {
        activeBehaviour?.SetSpeed(speed);
    }

    // Jump

    public void SetJumpBehaviour(JumpBehaviour behaviour)
    {
        if (jumpBehaviour != null && jumpBehaviour != behaviour)
            jumpBehaviour.Stop();

        jumpBehaviour = behaviour;
    }

    public bool Jump()
    {
        if (jumpBehaviour == null)
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
        if (action == null || startAction == null)
            return false;

        if (activeAction == action)
            return false;

        if (activeAction != null)
        {
            if (action.Priority <= activeAction.Priority)
                return false;

            activeAction.Stop();
            activeAction = null;
        }

        if (!startAction())
            return false;

        if (!action.IsActive)
            return false;

        activeAction = action;

        return true;
    }

    public void StopActiveAction()
    {
        if (activeAction == null)
            return;

        activeAction.Stop();
        activeAction = null;
    }

    // Update

    public void Update(float deltaTime)
    {
        if (activeAction != null)
        {
            activeAction.Update(deltaTime);

            if (activeAction.IsActive)
                return;

            activeAction = null;
        }

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
        Stop();

        jumpBehaviour?.Stop();
        jumpBehaviour = null;

        activeBehaviour = null;

        physicsSettings.RestoreOriginalSettings();
    }
}
