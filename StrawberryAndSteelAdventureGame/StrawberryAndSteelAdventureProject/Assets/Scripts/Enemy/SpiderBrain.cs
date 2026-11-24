using System.Collections.Generic;
using UnityEngine;

/// <summary>Spider-specific patrol, target, melee, special-attack and audio decisions.</summary>
public sealed class SpiderBrain : MonoBehaviour
{
    private enum BrainAnimation { None, Idle, Run, Attack, Hurt }

    [Header("References")]
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private ProjectileController projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("Patrol (local X coordinates)")]
    [SerializeField] private float patrolPointA = -2f;
    [SerializeField] private float patrolPointB = 2f;

    [Header("Visual")]
    [Tooltip("Sprite, localScale.x pozitifken saða bakýyorsa açýk býrak. Sola bakýyorsa kapat.")]
    [SerializeField] private bool spriteFacesRight = true;

    [Header("Combat")]
    [SerializeField, Min(0.05f)] private float attackCooldown = 1.5f;
    [Tooltip("Fallback minimum; the Animator clip completion is checked when available.")]
    [SerializeField, Min(0.05f)] private float attackAnimationDuration = 0.5f;
    [SerializeField, Min(0f)] private float specialAttackMinimumInterval = 4f;
    [SerializeField, Min(0f)] private float specialAttackMaximumInterval = 7f;
    [SerializeField, Range(0f, 100f)] private float specialAttackHealthThreshold = 50f;
    [SerializeField, Min(0f)] private float specialAttackRecoveryDuration = 0.75f;
    [SerializeField, Min(0f)] private float hurtDuration = 0.35f;
    [SerializeField, Min(0f)] private float stateAudioInterval = 5f;

    [Header("Debug")]
    [SerializeField] private bool debugLogs;

    private const float ArriveDistance = 0.08f;
    private const float FaceDeadZone = 0.05f;
    private const float MoveFaceThreshold = 0.002f;

    private EnemyMovement movement;
    private EnemyDetection detection;
    private EnemyHealth health;
    private EnemyCombat combat;
    private EnemyDialogue dialogue;

    private readonly List<ProjectileController> activeProjectiles = new List<ProjectileController>();

    private Vector3 startingLocalPosition;
    private EnemyState brainState;
    private BrainAnimation currentAnimation = BrainAnimation.None;
    private float attackTimer;
    private float attackAnimationTimer;
    private float specialAttackTimer;
    private float specialRecoveryTimer;
    private float audioTimer;
    private float hurtTimer;
    private float previousHealth;
    private float lastFaceX;
    private bool stateInitialized;
    private bool patrolStarted;
    private bool specialTimerStarted;
    private bool playerWasDetected;
    private bool returningToStart;
    private bool deathHandled;

    private void Start()
    {
        if (enemyController == null)
            enemyController = GetComponent<EnemyController>();

        if (enemyController == null)
        {
            Debug.LogError("SpiderBrain requires an EnemyController.", this);
            enabled = false;
            return;
        }

        movement = enemyController.Movement;
        detection = enemyController.Detection;
        health = enemyController.Health;
        combat = enemyController.Combat;
        dialogue = enemyController.Dialogue;

        if (movement == null || detection == null || health == null || combat == null)
        {
            Debug.LogError("SpiderBrain requires Movement, Detection, Health and Combat references on EnemyController.", this);
            enabled = false;
            return;
        }

        startingLocalPosition = transform.localPosition;
        lastFaceX = transform.position.x;
        previousHealth = health.CurrentHealth;
        specialAttackMaximumInterval = Mathf.Max(
            specialAttackMinimumInterval,
            specialAttackMaximumInterval);

        BindPlayerSession();
        StartPatrol();
        SetState(EnemyState.Patrol);
        PlayAnimation(BrainAnimation.Run);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < activeProjectiles.Count; i++)
        {
            if (activeProjectiles[i] != null)
                activeProjectiles[i].OnPlayerHit -= OnProjectileHit;
        }
        activeProjectiles.Clear();
    }

    private void Update()
    {
        if (enemyController == null || health == null || health.IsDead)
        {
            StopForDeath();
            return;
        }

        BindPlayerSession();

        float dt = Time.deltaTime;
        attackTimer = Mathf.Max(0f, attackTimer - dt);
        attackAnimationTimer = Mathf.Max(0f, attackAnimationTimer - dt);

        // Hasar alma: can her azaldýðýnda Hurt.
        if (health.CurrentHealth < previousHealth)
        {
            previousHealth = health.CurrentHealth;
            hurtTimer = hurtDuration;
            specialRecoveryTimer = 0f;
            attackAnimationTimer = 0f;
            StopMoving();
            PlayAnimation(BrainAnimation.Hurt, true);
            SetState(EnemyState.Hurt, true);
        }
        else
        {
            previousHealth = health.CurrentHealth;
        }

        if (hurtTimer > 0f)
        {
            hurtTimer -= dt;
            if (hurtTimer > 0f)
            {
                UpdateStateAudio();
                return;
            }
        }

        // Özel saldýrý sonrasý toparlanma: diðer kararlar çalýþmaz.
        // Keep one-shot actions visible until their Animator state completes.
        if (IsOneShotAnimationPending())
        {
            StopMoving();
            UpdateStateAudio();
            return;
        }

        if (specialRecoveryTimer > 0f)
        {
            specialRecoveryTimer -= dt;
            StopMoving();
            UpdateStateAudio();
            if (specialRecoveryTimer > 0f)
                return;
        }

        Transform player = detection.PlayerTransform;
        if (!detection.IsPlayerDetected || player == null)
        {
            if (playerWasDetected)
            {
                // Takip bitti: önce baþlangýç noktasýna dön, sonra devriyeye devam et.
                playerWasDetected = false;
                returningToStart = true;
            }

            specialTimerStarted = false;
            attackAnimationTimer = 0f;
            PatrolOrReturn();
            FaceMovementDirection();
            UpdateStateAudio();
            return;
        }

        playerWasDetected = true;
        returningToStart = false;
        patrolStarted = false;
        FacePlayer(player);
        lastFaceX = transform.position.x;

        if (TrySpecialAttack(player, dt))
        {
            UpdateStateAudio();
            return;
        }

        if (detection.IsPlayerInAttackRange)
        {
            StopMoving();

            bool attackReady = attackTimer <= 0f;
            // Yeni state'e girerken ses bir kez çalar; menzilde kalýrsak her vuruþta yeniden baþlar.
            SetState(EnemyState.Attack, attackReady);

            if (attackReady)
            {
                PlayAnimation(BrainAnimation.Attack, true);
                combat.Attack();
                attackTimer = Mathf.Max(0.05f, attackCooldown);
                attackAnimationTimer = attackAnimationDuration;
            }
            else
            {
                if (attackAnimationTimer <= 0f)
                    PlayAnimation(BrainAnimation.Idle); // sadece deðiþince çaðrýlýr
            }
        }
        else
        {
            attackAnimationTimer = 0f;
            float playerLocalX = transform.parent != null
                ? transform.parent.InverseTransformPoint(player.position).x
                : player.position.x;
            movement.Walk(playerLocalX);
            SetState(EnemyState.Chase);
            PlayAnimation(BrainAnimation.Run);
        }

        UpdateStateAudio();
    }

    private void BindPlayerSession()
    {
        GameBootstrapper bootstrapper = GameBootstrapper.Instance;
        if (bootstrapper != null && bootstrapper.PlayerCharacterSession != null)
            combat?.SetPlayerSession(bootstrapper.PlayerCharacterSession);
    }

    private void StopMoving()
    {
        movement.Stop(false, true);
        patrolStarted = false;
    }

    private bool IsOneShotAnimationPending()
    {
        EnemyAnimationController animation = enemyController.Animation;
        if (animation == null)
            return false;

        if (currentAnimation == BrainAnimation.Attack)
        {
            if (attackAnimationTimer > 0f)
                return true;

            return !animation.IsStateFinished("SpiderAttack");
        }

        if (currentAnimation == BrainAnimation.Hurt)
            return !animation.IsStateFinished("SpiderHurt");

        return false;
    }

    private void StartPatrol()
    {
        if (patrolStarted || movement == null)
            return;

        movement.WalkBetween(patrolPointA, patrolPointB);
        patrolStarted = true;
    }

    private void PatrolOrReturn()
    {
        SetState(EnemyState.Patrol);
        PlayAnimation(BrainAnimation.Run);

        if (returningToStart)
        {
            float distance = Mathf.Abs(transform.localPosition.x - startingLocalPosition.x);
            if (distance > ArriveDistance)
            {
                movement.Walk(startingLocalPosition.x);
                return;
            }

            returningToStart = false;
        }

        StartPatrol();
    }

    private bool TrySpecialAttack(Transform player, float dt)
    {
        if (projectilePrefab == null || projectileSpawnPoint == null || health.MaxHealth <= 0)
        {
            specialTimerStarted = false;
            return false;
        }

        float healthPercent = (float)health.CurrentHealth / health.MaxHealth * 100f;
        if (healthPercent > specialAttackHealthThreshold)
        {
            specialTimerStarted = false;
            return false;
        }

        if (!specialTimerStarted)
        {
            specialAttackTimer = Random.Range(
                specialAttackMinimumInterval,
                specialAttackMaximumInterval);
            specialTimerStarted = true;
        }

        specialAttackTimer -= dt;
        if (specialAttackTimer > 0f)
            return false;

        Vector2 direction = (Vector2)(player.position - projectileSpawnPoint.position);
        if (direction.sqrMagnitude <= 0.0001f)
        {
            specialTimerStarted = false;
            return false;
        }

        StopMoving();
        attackAnimationTimer = 0f;
        SetState(EnemyState.SpecialAttack, true);
        PlayAnimation(BrainAnimation.Attack, true);

        ProjectileController projectile = Instantiate(
            projectilePrefab,
            projectileSpawnPoint.position,
            Quaternion.identity);

        if (projectile != null)
        {
            projectile.OnPlayerHit += OnProjectileHit;
            activeProjectiles.Add(projectile);
            projectile.Launch(direction.normalized);
        }

        specialRecoveryTimer = specialAttackRecoveryDuration;
        specialTimerStarted = false;
        return true;
    }

    private void OnProjectileHit(ProjectileDefinition definition, Collider2D playerCollider)
    {
        if (health == null || health.IsDead || combat == null ||
            definition == null || playerCollider == null ||
            enemyController == null || enemyController.Definition == null)
        {
            return;
        }

        int damage = Mathf.Max(1, Mathf.RoundToInt(
            enemyController.Definition.AttackDamage * definition.DamageMultiplier));
        combat.SpecialAttack(damage);
    }

    // ---------- Yön ----------

    private void FacePlayer(Transform player)
    {
        float dx = player.position.x - transform.position.x;
        if (Mathf.Abs(dx) < FaceDeadZone)
            return; // oyuncu tam üstündeyken titremeyi önler

        SetFacing(dx > 0f);
    }

    /// <summary>Devriye / baþlangýca dönüþ sýrasýnda gerçek hareket yönüne bakar.</summary>
    private void FaceMovementDirection()
    {
        float x = transform.position.x;
        float dx = x - lastFaceX;

        // Rigidbody2D fizik adýmýnda hareket eder; hareket yoksa yönü deðiþtirme.
        if (Mathf.Abs(dx) < MoveFaceThreshold)
            return;

        lastFaceX = x;
        SetFacing(dx > 0f);
    }

    private void SetFacing(bool faceRight)
    {
        Vector3 scale = transform.localScale;
        float sign = faceRight == spriteFacesRight ? 1f : -1f;
        scale.x = Mathf.Abs(scale.x) * sign;
        transform.localScale = scale;
    }

    // ---------- State, ses, animasyon ----------

    /// <summary>
    /// State'i brain'in kendi takibiyle deðiþtirir. State deðiþince (veya restartSound true ise)
    /// EnemyDialogue'a "kes ve bu state'in sesini çal" komutu gönderilir.
    /// </summary>
    private void SetState(EnemyState state, bool restartSound = false)
    {
        if (enemyController.CurrentState != state)
            enemyController.SetState(state);

        bool changed = !stateInitialized || brainState != state;
        if (!changed && !restartSound)
            return;

        if (changed && debugLogs)
            Debug.Log($"[SpiderBrain] State: {(stateInitialized ? brainState.ToString() : "-")} -> {state}", this);

        stateInitialized = true;
        brainState = state;
        audioTimer = stateAudioInterval;
        dialogue?.Speak(state, true);
    }

    private void PlayAnimation(BrainAnimation kind, bool restart = false)
    {
        var animation = enemyController.Animation;
        if (animation == null)
            return;

        // Run her karede yeniden set edilir (bool parametre, zararsýz); diðerleri sadece deðiþince.
        if (!restart && kind != BrainAnimation.Run && currentAnimation == kind)
            return;

        if (debugLogs && currentAnimation != kind)
            Debug.Log($"[SpiderBrain] Animation: {currentAnimation} -> {kind}", this);

        currentAnimation = kind;

        switch (kind)
        {
            case BrainAnimation.Idle: animation.PlayIdle(); break;
            case BrainAnimation.Run: animation.PlayRun(); break;
            case BrainAnimation.Attack: animation.PlayAttack(); break;
            case BrainAnimation.Hurt: animation.PlayHurt(); break;
        }
    }

    private void UpdateStateAudio()
    {
        if (dialogue == null || stateAudioInterval <= 0f)
            return;

        audioTimer -= Time.deltaTime;
        if (audioTimer > 0f)
            return;

        dialogue.Speak(brainState);
        audioTimer = stateAudioInterval;
    }

    private void StopForDeath()
    {
        if (deathHandled)
            return;

        deathHandled = true;

        if (movement != null)
            movement.Stop(false, true);

        if (enemyController != null)
        {
            enemyController.SetState(EnemyState.Death);
            enemyController.Animation?.PlayDeath();
            dialogue?.Speak(EnemyState.Death, true);

            if (debugLogs)
                Debug.Log("[SpiderBrain] Death", this);
        }
    }

    private void OnValidate()
    {
        attackCooldown = Mathf.Max(0.05f, attackCooldown);
        attackAnimationDuration = Mathf.Max(0.05f, attackAnimationDuration);
        specialAttackMinimumInterval = Mathf.Max(0f, specialAttackMinimumInterval);
        specialAttackMaximumInterval = Mathf.Max(specialAttackMinimumInterval, specialAttackMaximumInterval);
        specialAttackHealthThreshold = Mathf.Clamp(specialAttackHealthThreshold, 0f, 100f);
        specialAttackRecoveryDuration = Mathf.Max(0f, specialAttackRecoveryDuration);
        hurtDuration = Mathf.Max(0f, hurtDuration);
        stateAudioInterval = Mathf.Max(0f, stateAudioInterval);
    }
}