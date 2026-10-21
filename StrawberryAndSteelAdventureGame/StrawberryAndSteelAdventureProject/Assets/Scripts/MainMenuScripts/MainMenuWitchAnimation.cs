using UnityEngine;

public class MainMenuWitchAnimation : MonoBehaviour
{
    private const float MovementSpeed = 5f;
    private const float StartX = -22f;
    private const float EndX = 22f;
    private const float MinY = 4f;
    private const float MaxY = 5f;

    private WitchController witchController;
    private bool initialized;
    private bool movingRight;
    private float startY;
    private float destinationY;
    private bool destroyScheduled;

    public void Initialize(bool movingRight, float startY, float destinationY)
    {
        this.movingRight = movingRight;
        this.startY = startY;
        this.destinationY = destinationY;
        initialized = true;
    }

    private void Start()
    {
        Animator animator = GetComponent<Animator>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null || spriteRenderer == null)
        {
            Debug.LogError("MainMenuWitchAnimation requires an Animator and SpriteRenderer.", this);
            enabled = false;
            return;
        }

        spriteRenderer.sortingOrder = 5;
        witchController = new WitchController(transform, animator, spriteRenderer, MovementSpeed);
        if (!initialized)
        {
            movingRight = Random.value < 0.5f;
            startY = Random.Range(MinY, MaxY);
            destinationY = Random.Range(MinY, MaxY);
            transform.localPosition = new Vector3(movingRight ? StartX : EndX,
                startY, transform.localPosition.z);
        }
        witchController.MoveTo(new Vector2(movingRight ? EndX : StartX, destinationY),
            "FlyWithBroom");
    }

    private void Update()
    {
        if (witchController == null || destroyScheduled) return;
        witchController.Tick(Time.deltaTime);
        if (!witchController.IsMoving)
        {
            destroyScheduled = true;
            Destroy(gameObject, 0.25f);
        }
    }
}
