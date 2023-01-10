using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Rope : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private GameObject ropeTrailParticlePrefab;

    private LineRenderer lineRenderer;
    private IPositionLocalizer positionLocalizer;

    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    private float distanceBetweenSegment = 0.05f;
    private int segmentCount = 80;
    private float lineWidth = 0.1f;

    private float restoreForceAdjustment;
    private float ropeGravityAdjustment;
    private float ropeLengthAdjustment;

    public Vector3 ropeStartPoint { get; private set; }
    private Vector3 ropeStartPointLocal;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        for (int i = 0; i < segmentCount; i++)
        {
            ropeSegments.Add(new RopeSegment(i * Vector2.down * distanceBetweenSegment));
        }
    }

    private void Update()
    {
        DrawRope();
    }

    private void FixedUpdate()
    {
        Simulate();
    }

    private void OnEnable()
    {
        lineRenderer.enabled = true;
    }

    private void OnDisable()
    {
        lineRenderer.enabled = false;
    }

    public void Hang(Vector3 hitPos, Vector3 playerPos, string hitTerrainType, IPositionLocalizer positionLocalizer)
    {
        SetAdjustmentByTerrainType(hitTerrainType);

        ropeStartPoint = hitPos;
        distanceBetweenSegment = (hitPos - playerPos).magnitude / segmentCount * ropeLengthAdjustment;
        SetPositionLocalizer(positionLocalizer);

        StretchRope(hitPos, playerPos);
        DrawRope();
    }

    private void SetPositionLocalizer(IPositionLocalizer positionLocalizer)
    {
        this.positionLocalizer = positionLocalizer;

        if (positionLocalizer != null)
        {
            ropeStartPointLocal = positionLocalizer.WorldToLocalPos(ropeStartPoint);
        }
    }

    private void Simulate()
    {
        // SIMULATION
        Vector2 gravity = Vector2.down * ropeGravityAdjustment;

        for(int i = 0; i < segmentCount; i++)
        {
            RopeSegment segment = ropeSegments[i];
            Vector2 velocity = segment.posNow - segment.posOld;

            segment.posOld = segment.posNow;
            segment.posNow += velocity * 1f;
            segment.posNow += gravity * Time.deltaTime;

            ropeSegments[i] = segment;
        }

        // CONSTRAINTS
        SetRopeStartPointByLocal();

        for(int i = 0; i < 100; i++)
        {
            ApplyConstraint();
        }
    }

    private void ApplyConstraint()
    {
        RopeSegment firstSegment = ropeSegments[0];
        firstSegment.posNow = ropeStartPoint;
        ropeSegments[0] = firstSegment;

        for(int i = 0; i < segmentCount - 1; i++)
        {
            RopeSegment leadingSegment = ropeSegments[i];
            RopeSegment followingSegment = ropeSegments[i + 1];

            float distance = (leadingSegment.posNow - followingSegment.posNow).magnitude;
            float error = distance - distanceBetweenSegment;

            Vector2 correction = error * (leadingSegment.posNow - followingSegment.posNow).normalized;
            
            if(i != 0)
            {
                leadingSegment.posNow -= correction * 0.5f;
                followingSegment.posNow += correction * 0.5f;
            }
            else
            {
                followingSegment.posNow += correction;
            }

            ropeSegments[i] = leadingSegment;
            ropeSegments[i + 1] = followingSegment;
        }

        RopeSegment lastSegment = ropeSegments[ropeSegments.Count - 1];
        float distanceError =
            (firstSegment.posNow - (Vector2)playerTransform.position).magnitude - distanceBetweenSegment * (segmentCount - 1);
        float limitation = 2f;
        float restoreForce = distanceError * restoreForceAdjustment <= limitation ?
            distanceError * restoreForceAdjustment : limitation;

        if (distanceError > 0)
        {
            playerTransform.GetComponent<Rigidbody2D>().AddForce( restoreForce * (firstSegment.posNow - (Vector2)playerTransform.position).normalized );
        }

        lastSegment.posNow = (Vector2)playerTransform.position;
        ropeSegments[ropeSegments.Count - 1] = lastSegment;
    }

    private void DrawRope()
    {
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[segmentCount];
        for(int i = 0; i < segmentCount - 1; i++)
        {
            ropePositions[i] = ropeSegments[i].posNow;
        }

        // 그리기 보정 (플레이어가 빠르게 움직일 때 밧줄에서 벗어나 보이는 문제를 해결)
        int drawAdjustment = 6; // 보정할 ropeSegment의 개수
        ropePositions[segmentCount - 1] = playerTransform.position;
        for (int i = 2; i <= drawAdjustment; i++)
        {
            ropePositions[segmentCount - i] = MathUtility.GetInternalDividingPoint(
                                                            ropePositions[segmentCount - i + 1],
                                                            ropePositions[segmentCount - i - 1],
                                                            1, 1);
        }

        // lineRenderer 적용
        lineRenderer.positionCount = segmentCount;
        lineRenderer.SetPositions(ropePositions);
    }

    private void SetRopeStartPointByLocal()
    {
        if (positionLocalizer == null)
        {
            return;
        }

        ropeStartPoint = positionLocalizer.LocalToWorldPos(ropeStartPointLocal);
    }

    private void SetAdjustmentByTerrainType(string name)
    {
        switch (name)
        {
            case "ground":
                restoreForceAdjustment = 0.4f;
                ropeGravityAdjustment = 3f;
                ropeLengthAdjustment = 0.4f;
                break;

            case "obstacle":
                restoreForceAdjustment = 0.4f;
                ropeGravityAdjustment = 3f;
                ropeLengthAdjustment = 0.4f;
                break;

            case "hook":
                restoreForceAdjustment = 0.6f;
                ropeGravityAdjustment = 0f;
                ropeLengthAdjustment = 0.4f;
                break;

            case "guard":
                restoreForceAdjustment = 0.4f;
                ropeGravityAdjustment = 3f;
                ropeLengthAdjustment = 0.6f;
                break;

            case "magnet":
                restoreForceAdjustment = 0.4f;
                ropeGravityAdjustment = 3f;
                ropeLengthAdjustment = 0.4f;
                break;

            default:
                Debug.LogWarning("Rope: 등록되지 않은 지형입니다!");
                break;
        }
    }

    private void StretchRope(Vector3 hitPos, Vector3 playerPos)
    {
        for (int i = 0; i < segmentCount; i++)
        {
            RopeSegment segment = new RopeSegment();

            segment.posNow = MathUtility.GetInternalDividingPoint(hitPos, playerPos, i, (segmentCount - 1) - i);
            segment.posOld = segment.posNow;

            ropeSegments[i] = segment;
        }
    }

    public struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            posNow = pos;
            posOld = pos;
        }
    }

    public void PlayTrailParticle()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            ParticleManager.instance.PlayParticle(ropeTrailParticlePrefab, ropeSegments[i].posNow);
        }
    }
}
