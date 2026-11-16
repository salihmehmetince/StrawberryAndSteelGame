
using System;

[Flags]
public enum EnemyCapability
{
    None = 0,

    // Movement
    GroundMovement = 1 << 0,
    Flying = 1 << 1,
    Swimming = 1 << 2,
    Jumping = 1 << 3,
    Running = 1 << 4,
    Dashing = 1 << 5,
    Charging = 1 << 6,

    // Combat
    MeleeCombat = 1 << 7,
    RangedCombat = 1 << 8,
    AreaCombat = 1 << 9,
    DiveAttack = 1 << 10,

    // Status Effects
    Invisibility = 1 << 11,
    Poison = 1 << 12,
    Burning = 1 << 13,
    Freezing = 1 << 14,
    Stunning = 1 << 15,

    // Defense
    Shielding = 1 << 16,
    Blocking = 1 << 17,
    Regeneration = 1 << 18,

    // Special
    Teleportation = 1 << 19,
    Summoning = 1 << 20,
    Transformation = 1 << 21,
    Healing = 1 << 22
}
