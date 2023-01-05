using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerRopeShooter : MonoBehaviour
{
    [SerializeField] private Rope rope;
    [SerializeField] private CameraMover cameraMover;
    [SerializeField] private GameObject ropeShootParticlePrefab;

    [HideInInspector] public TimeMeasurer timeMeasurer;

    public enum RaycastResult
    {
        RopeableObject,
        NotRopeableObject,
        NoObject
    }

    private PlayerInput playerInput;
    private PlayerMover playerMover;

    private float lastRopeShootTime;
    
    public int numRopeUsedToClear { get; private set; }
    private float _secondsTakenToClear;
    public float secondsTakenToClear
    {
        get
        {
            if (GameManager.instance.gameState == GameManager.GameState.Cleared)
            {
                return _secondsTakenToClear;
            }

            return timeMeasurer.CheckMeasure();
        }

        private set
        {
            _secondsTakenToClear = value;
        }
    }

    private IRopeable ropeHangObject;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerMover = GetComponent<PlayerMover>();

        playerInput.OnMouseLeftClick += TryToShootRope;
        playerInput.OnMouseRightClick += CutRope;

        numRopeUsedToClear = 0;
        lastRopeShootTime = 0f;

        timeMeasurer = new TimeMeasurer();
    }

    private void Update()
    {
        if(cameraMover.targetMode == CameraMover.TargetMode.SectionSnapped
            && cameraMover.HasTargetPositionChanged())
        {
            CutRope(Vector3.zero);
        }
    }

    private void TryToShootRope(Vector3 clickPos)
    {
        IRopeable ropeable;
        IPositionLocalizer positionLocalizer;
        RaycastHit2D hit;

        float coolTime = 0.3f;

        // 쿨타임 설정
        if (Time.time < lastRopeShootTime + coolTime)
        {
            return;
        }

        switch (PerformRaycast(clickPos, out ropeable, out positionLocalizer, out hit))
        {
            case RaycastResult.RopeableObject:
                ParticleManager.instance.PlayParticle(ropeShootParticlePrefab, hit.point);
                ShootRope(hit, ropeable, positionLocalizer);
                ChangePlayerMovementByTerrainType(ropeable.GetTerrainType());
                break;

            case RaycastResult.NotRopeableObject:
                ParticleManager.instance.PlayParticle(ropeShootParticlePrefab, hit.point);
                break;

            case RaycastResult.NoObject:
                break;
        }
    }

    private void ShootRope(RaycastHit2D hit, IRopeable ropeable, IPositionLocalizer positionLocalizer)
    {
        if (rope.enabled == true)
        {
            rope.PlayTrailParticle();
        }

        rope.Hang(hit.point, transform.position, ropeable.GetTerrainType(), positionLocalizer);
        rope.enabled = true;

        lastRopeShootTime = Time.time;
        IncreaseNumRopeUsedToClear();
        StartMeasureTimeTakenAfterFirstRopeUsed();

        SetRopeHangObject(ropeable);
        ropeHangObject.OnRopeHang();
    }

    public void CutRope(Vector3 clickPos)
    {
        if (rope.enabled == false)
        {
            return;
        }

        rope.enabled = false;

        playerMover.ResetPlayerMovement();
        rope.PlayTrailParticle();

        ropeHangObject.OnRopeCut();
        SetRopeHangObject(null);
    }

    private void ChangePlayerMovementByTerrainType(string name)
    {
        playerMover.isMoveable = false;

        switch (name)
        {
            case "ground":
                playerMover.StunInAir();
                playerMover.SetPlayerGravity(2f);
                break;

            case "obstacle":
                playerMover.StunInAir();
                playerMover.SetPlayerGravity(2f);
                break;

            case "hook":
                playerMover.StunInAir();
                playerMover.SetPlayerGravity(0f);
                break;

            case "guard":
                playerMover.StunInAir();
                playerMover.SetPlayerGravity(2f);
                break;

            default:
                Debug.LogWarning("PlayerRopeShooter: 등록되지 않은 지형입니다!");
                break;
        }
    }

    private void SetRopeHangObject(IRopeable ropeable)
    {
        ropeHangObject = ropeable;
    }

    public bool IsRopeShooted()
    {
        return rope.enabled;
    }

    private void IncreaseNumRopeUsedToClear()
    {
        if (numRopeUsedToClear < 999)
        {
            numRopeUsedToClear++;
        }
    }

    private void StartMeasureTimeTakenAfterFirstRopeUsed()
    {
        if (numRopeUsedToClear == 1)
        {
            timeMeasurer.StartMeasure();
        }
    }

    public void StopAndRecordTimeTaken()
    {
        secondsTakenToClear = timeMeasurer.StopMeasure();
    }

    public RaycastResult PerformRaycast(Vector3 endPos)
    {
        Vector3 playerPos = transform.position + (endPos - transform.position).normalized * (transform.localScale.x * 0.5f + 0.01f);
        float outerThreshold = 0f;

        RaycastHit2D hit = Physics2D.Raycast(playerPos, (endPos - playerPos).normalized);

        if (hit.collider != null)
        {
            if (cameraMover.isOutOfCamera(hit.point, outerThreshold))
            {
                return RaycastResult.NoObject;
            }

            IRopeable ropeable = hit.collider.GetComponent<IRopeable>();

            if (ropeable == null)
            {
                return RaycastResult.NotRopeableObject;
            }

            return RaycastResult.RopeableObject;
        }

        return RaycastResult.NoObject;
    }

    private RaycastResult PerformRaycast(Vector3 endPos, out IRopeable ropeableResult, out IPositionLocalizer positionLocalizerResult, out RaycastHit2D hitResult)
    {
        Vector3 playerPos = transform.position + (endPos - transform.position).normalized * (transform.localScale.x * 0.5f + 0.01f);
        float outerThreshold = 0f;

        RaycastHit2D hit = Physics2D.Raycast(playerPos, (endPos - playerPos).normalized);

        hitResult = hit;

        if (hit.collider != null)
        {
            if (cameraMover.isOutOfCamera(hit.point, outerThreshold))
            {
                ropeableResult = null;
                positionLocalizerResult = null;
                return RaycastResult.NoObject;
            }

            IRopeable ropeable = hit.collider.GetComponent<IRopeable>();

            if (ropeable == null)
            {
                ropeableResult = null;
                positionLocalizerResult = null;
                return RaycastResult.NotRopeableObject;
            }

            ropeableResult = ropeable;
            positionLocalizerResult = hit.collider.GetComponent<IPositionLocalizer>();
            return RaycastResult.RopeableObject;
        }

        ropeableResult = null;
        positionLocalizerResult = null;
        return RaycastResult.NoObject;
    }

}
