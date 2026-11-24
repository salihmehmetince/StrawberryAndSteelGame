
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class EnemyDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float attackRadius = 1.5f;
    [SerializeField] private LayerMask playerLayer;

    private CircleCollider2D detectionCollider;
    private Transform playerTransform;

    public bool IsPlayerDetected => playerTransform != null;

    public bool IsPlayerInAttackRange =>
        playerTransform != null &&
        Vector2.Distance(transform.position, playerTransform.position) <= attackRadius;

    public Transform PlayerTransform => playerTransform;

    public float PlayerDistance =>
        playerTransform != null
            ? Vector2.Distance(transform.position, playerTransform.position)
            : Mathf.Infinity;

    private void Awake()
    {
        detectionCollider = GetComponent<CircleCollider2D>();

        detectionCollider.isTrigger = true;
        detectionCollider.radius = detectionRadius;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDetectPlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDetectPlayer(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Transform detectedTransform = GetPlayerTransform(other);

        if (detectedTransform == playerTransform)
            playerTransform = null;
    }

    private void TryDetectPlayer(Collider2D other)
    {
        int objectLayer = other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject.layer
            : other.gameObject.layer;

        bool isOnPlayerLayer = (playerLayer.value & (1 << objectLayer)) != 0;
        bool hasPlayerTag = other.CompareTag("Player") ||
            (other.attachedRigidbody != null &&
             other.attachedRigidbody.CompareTag("Player")) ||
            other.transform.root.CompareTag("Player");

        // The player prefab can put its Rigidbody2D on a Default-layer root
        // while the Player tag is on the root/child. Accept either setup.
        if (!isOnPlayerLayer && !hasPlayerTag)
            return;

        playerTransform = GetPlayerTransform(other);
    }

    private Transform GetPlayerTransform(Collider2D other)
    {
        if (other.attachedRigidbody != null)
            return other.attachedRigidbody.transform;

        return other.transform.root;
    }

    private void OnDisable()
    {
        playerTransform = null;
    }

    private void OnValidate()
    {
        detectionRadius = Mathf.Max(0f, detectionRadius);
        attackRadius = Mathf.Clamp(attackRadius, 0f, detectionRadius);

        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();

        if (circleCollider != null)
        {
            circleCollider.isTrigger = true;
            circleCollider.radius = detectionRadius;
        }
    }
}
