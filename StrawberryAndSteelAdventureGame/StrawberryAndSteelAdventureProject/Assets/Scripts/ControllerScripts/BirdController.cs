using UnityEngine;

public class BirdControl
{
    private readonly Transform birdTransform;
    private readonly SpriteRenderer spriteRenderer;
    private readonly ParticleSystem birdTrail;
    private readonly Transform birdTrailTransform;

    public float MovementSpeed { get; set; }
    public float StartX { get; set; }
    public float EndX { get; set; }
    public float MinY { get; set; }
    public float MaxY { get; set; }

    public float SineWaveAmplitude { get; set; }
    public float SineWaveFrequency { get; set; }

    public bool IsFlying { get; private set; }

    private const float SpeedMultiplier = 0.2f;
    private const float CircleRadiusInUnits = 0.25f;
    private const float RotationSpeed = 360f;

    private enum FlightType
    {
        None,
        Straight,
        Circle,
        SineWave
    }

    private FlightType currentFlightType;

    private bool movingRight;
    private Vector2 targetPosition;
    private Vector2 startPosition;
    private Vector2 circleCenter;

    private float traveledDistance;
    private float totalDistance;
    private float circleAngle;
    private float circleStartDistance;
    private float totalFlightDistance;

    public BirdControl(
        Transform birdTransform,
        SpriteRenderer spriteRenderer)
    {
        this.birdTransform = birdTransform;
        this.spriteRenderer = spriteRenderer;

        GameObject trailObject = new GameObject("BirdTrail");
        trailObject.transform.SetParent(birdTransform, false);
        trailObject.transform.localPosition = Vector3.zero;
        birdTrailTransform = trailObject.transform;

        birdTrail = trailObject.AddComponent<ParticleSystem>();

        ConfigureBirdTrail();
    }

    private void ConfigureBirdTrail()
    {
        var main = birdTrail.main;
        main.playOnAwake = false;
        main.loop = true;
        main.startLifetime = 1.2f;
        main.startSpeed = 0f;
        main.startSize = 0.12f;
        main.startColor = Color.white;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = birdTrail.emission;
        emission.enabled = true;
        emission.rateOverTime = 150f;

        var shape = birdTrail.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.05f;

        var sizeOverLifetime = birdTrail.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(
            1f,
            new AnimationCurve(
                new Keyframe(0f, 1f),
                new Keyframe(1f, 0f)
            )
        );

        var colorOverLifetime = birdTrail.colorOverLifetime;
        colorOverLifetime.enabled = true;

        Gradient gradient = new Gradient();

        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );

        colorOverLifetime.color =
            new ParticleSystem.MinMaxGradient(gradient);

        var particleRenderer =
            birdTrail.GetComponent<ParticleSystemRenderer>();

        particleRenderer.renderMode = ParticleSystemRenderMode.Billboard;
        particleRenderer.sortingOrder = 5;
        if (particleRenderer.material != null)
            particleRenderer.material.color = Color.white;

        birdTrail.Stop();
        birdTrail.Clear();
    }

    public void FlyTo(Vector2 destination)
    {
        StartFlight(destination, FlightType.Straight);
    }

    public void FlyWithCircleTo(Vector2 destination)
    {
        StartFlight(destination, FlightType.Circle);
    }

    public void FlyWithSineWaveTo(Vector2 destination)
    {
        StartFlight(destination, FlightType.SineWave);
    }

    private void StartFlight(Vector2 destination, FlightType flightType)
    {
        StopFlying();

        startPosition = CurrentPosition;
        targetPosition = destination;

        Vector2 displacement = targetPosition - startPosition;
        totalDistance = displacement.magnitude;

        if (totalDistance <= 0.001f || MovementSpeed <= 0f)
            return;

        movingRight = displacement.x >= 0f;
        birdTrailTransform.localPosition = new Vector3(
            movingRight ? -0.2f : 0.2f,
            0f,
            0f
        );

        traveledDistance = 0f;
        circleAngle = 0f;

        currentFlightType = flightType;
        IsFlying = true;

        spriteRenderer.flipX = !movingRight;
        spriteRenderer.enabled = true;

        birdTrail.Clear();
        birdTrail.Play();

        if (flightType == FlightType.Circle)
        {
            Vector2 forward = displacement.normalized;
            Vector2 perpendicular = GetCirclePerpendicular(forward);
            circleStartDistance = totalDistance * 0.5f;
            circleCenter = startPosition + forward * circleStartDistance
                + perpendicular * CircleRadiusInUnits;
            totalFlightDistance = totalDistance + Mathf.PI * 2f * CircleRadiusInUnits;
        }
    }

    public void Tick(float deltaTime)
    {
        if (!IsFlying)
            return;

        switch (currentFlightType)
        {
            case FlightType.Straight:
                UpdateStraightFlight(deltaTime);
                break;

            case FlightType.Circle:
                UpdateCircleFlight(deltaTime);
                break;

            case FlightType.SineWave:
                UpdateSineWaveFlight(deltaTime);
                break;
        }
    }

    private void UpdateStraightFlight(float deltaTime)
    {
        Vector2 previousPosition = CurrentPosition;

        Vector2 newPosition = Vector2.MoveTowards(
            previousPosition,
            targetPosition,
            MovementSpeed * SpeedMultiplier * deltaTime
        );

        SetLocalPosition(newPosition);

        UpdateRotation(newPosition - previousPosition, deltaTime);

        if (Vector2.Distance(newPosition, targetPosition) <= 0.001f)
        {
            SetLocalPosition(targetPosition);
            StopFlying();
        }
    }

    private void UpdateCircleFlight(float deltaTime)
    {
        traveledDistance = Mathf.Min(
            traveledDistance + MovementSpeed * SpeedMultiplier * deltaTime,
            totalFlightDistance
        );

        Vector2 previousPosition = CurrentPosition;
        Vector2 newPosition;
        Vector2 forward = (targetPosition - startPosition).normalized;
        Vector2 perpendicular = GetCirclePerpendicular(forward);

        if (traveledDistance < circleStartDistance)
        {
            newPosition = startPosition + forward * traveledDistance;
        }
        else if (traveledDistance <= circleStartDistance + Mathf.PI * 2f * CircleRadiusInUnits)
        {
            circleAngle = (traveledDistance - circleStartDistance) / CircleRadiusInUnits;
            newPosition = circleCenter
                + forward * (Mathf.Sin(circleAngle) * CircleRadiusInUnits)
                - perpendicular * (Mathf.Cos(circleAngle) * CircleRadiusInUnits);
        }
        else
        {
            float distanceAfterCircle = traveledDistance
                - circleStartDistance - Mathf.PI * 2f * CircleRadiusInUnits;
            newPosition = startPosition + forward * (circleStartDistance + distanceAfterCircle);
        }

        SetLocalPosition(newPosition);
        Vector2 tangent = newPosition - previousPosition;
        if (traveledDistance >= circleStartDistance
            && traveledDistance <= circleStartDistance + Mathf.PI * 2f * CircleRadiusInUnits)
        {
            tangent = forward * Mathf.Cos(circleAngle) + perpendicular * Mathf.Sin(circleAngle);
        }
        UpdateRotation(tangent, deltaTime);

        if (traveledDistance >= totalFlightDistance)
        {
            SetLocalPosition(targetPosition);
            StopFlying();
        }
    }

    private static Vector2 GetCirclePerpendicular(Vector2 forward)
    {
        Vector2 perpendicular = new Vector2(-forward.y, forward.x);
        if (perpendicular.y < 0f)
            perpendicular = -perpendicular;

        return perpendicular;
    }

    private void UpdateSineWaveFlight(float deltaTime)
    {
        traveledDistance += MovementSpeed * SpeedMultiplier * deltaTime;

        float progress = Mathf.Clamp01(traveledDistance / totalDistance);

        Vector2 basePosition = Vector2.Lerp(
            startPosition,
            targetPosition,
            progress
        );

        Vector2 direction = (targetPosition - startPosition).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        float waveAngle = progress * Mathf.Max(0f, SineWaveFrequency) * Mathf.PI * 2f;
        float waveOffset = Mathf.Sin(waveAngle) * SineWaveAmplitude;

        Vector2 newPosition = basePosition + perpendicular * waveOffset;

        SetLocalPosition(newPosition);

        Vector2 tangent = direction + perpendicular * (
            SineWaveAmplitude * Mathf.Max(0f, SineWaveFrequency) * Mathf.PI * 2f * Mathf.Cos(waveAngle) / totalDistance
        );
        UpdateRotation(tangent, deltaTime);

        if (progress >= 1f)
        {
            SetLocalPosition(targetPosition);
            StopFlying();
        }
    }

    private void UpdateRotation(Vector2 direction, float deltaTime)
    {
        if (direction.sqrMagnitude <= 0.0001f)
            return;

        float angle = Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg;

        // flipX, yerel sağ yönünü sola çevirdiği için sola uçuşta
        // gövde rotasyonundan 180 derece çıkarılmalı.
        if (!movingRight)
            angle -= 180f;

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

        birdTransform.localRotation = Quaternion.RotateTowards(
            birdTransform.localRotation,
            targetRotation,
            RotationSpeed * deltaTime
        );
    }

    private Vector2 CurrentPosition =>
        new Vector2(
            birdTransform.localPosition.x,
            birdTransform.localPosition.y
        );

    private void SetLocalPosition(Vector2 position)
    {
        birdTransform.localPosition = new Vector3(
            position.x,
            position.y,
            birdTransform.localPosition.z
        );
    }

    public void StopFlying()
    {
        IsFlying = false;
        currentFlightType = FlightType.None;

        if (birdTrail != null)
        {
            birdTrail.Stop();
            birdTrail.Clear();
        }
    }
}
