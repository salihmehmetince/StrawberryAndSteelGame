
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform background;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float backgroundSmoothSpeed = 3f;

    private Vector3 previousCameraPosition;
    private Vector3 backgroundTargetPosition;

    private void Start()
    {
        previousCameraPosition = transform.position;

        if (background != null)
            backgroundTargetPosition = background.position;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );

        Vector3 cameraMovement =
            transform.position - previousCameraPosition;

        if (background != null)
        {
            backgroundTargetPosition += cameraMovement;

            background.position = Vector3.Lerp(
                background.position,
                backgroundTargetPosition,
                backgroundSmoothSpeed * Time.deltaTime
            );
        }

        previousCameraPosition = transform.position;
    }
}
