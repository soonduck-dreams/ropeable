using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerRopeShooter : MonoBehaviour
{
    [SerializeField]
    private Rope rope;

    [SerializeField]
    private CameraMover cameraMover;

    [SerializeField]
    private GameObject ropeShootParticlePrefab;

    [HideInInspector]
    public TimeMeasurer timeMeasurer;

    private PlayerInput playerInput;
    private PlayerMover playerMover;

    private float lastRopeShootTime;
    
    public int numRopeUsedToClear { get; private set; }
    public float secondsTakenToClear { get; private set; }

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
        Vector3 playerPos = transform.position + (clickPos - transform.position).normalized * (transform.localScale.x * 0.5f + 0.01f);
        float outerThreshold = 0f;
        float coolTime = 0.3f;

        // 쿨타임 설정
        if (Time.time < lastRopeShootTime + coolTime)
        {
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(playerPos, (clickPos - playerPos).normalized);
        if (hit.collider != null)
        {
            IRopeable ropeable = hit.collider.GetComponent<IRopeable>();
            IPositionLocalizer positionLocalizer = hit.collider.GetComponent<IPositionLocalizer>();

            if (cameraMover.isOutOfCamera(hit.point, outerThreshold))
            {
                return;
            }

            ParticleManager.instance.PlayParticle(ropeShootParticlePrefab, hit.point);

            if (ropeable != null)
            {
                ShootRope(hit, ropeable, positionLocalizer);
                ChangePlayerMovementByTerrainType(ropeable.GetTerrainType());
            }
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

    // deprecated
    private void PlayRopeShootParticle(RaycastHit2D hit)
    {
        GameObject particleObject = Instantiate(ropeShootParticlePrefab, hit.point, Quaternion.identity);
        ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();

        particleSystem.Play();
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
}
