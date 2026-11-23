
using UnityEngine;

public class EnemySpecialAttackRuntime
{
    public EnemySpecialAttackDefinition Definition { get; private set; }

    public float RemainingCooldown { get; private set; }

    public bool IsReady => RemainingCooldown <= 0f;

    public EnemySpecialAttackRuntime(
        EnemySpecialAttackDefinition definition)
    {
        Definition = definition;
        RemainingCooldown = 0f;
    }

    public void StartCooldown()
    {
        if (Definition == null)
            return;

        RemainingCooldown = Definition.Cooldown;
    }

    public void Update(float deltaTime)
    {
        if (RemainingCooldown <= 0f)
            return;

        RemainingCooldown -= Mathf.Max(0f, deltaTime);

        if (RemainingCooldown < 0f)
            RemainingCooldown = 0f;
    }
}
