using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField]
    private GameObject playerRocketParticlePrefab;

    [SerializeField]
    private GameObject playerTrailParticlePrefab;

    private float moveSpeedHorizontal;
    private float basicGravity;

    private bool _isMoveable;
    public bool isMoveable
    {
        get
        {
            return _isMoveable;
        }

        set
        {
            _isMoveable = value;
            
        }
    }

    private Rigidbody2D playerRigidbody;
    private PlayerInput playerInput;
    private PlayerRopeShooter playerRopeShooter;
    private SpriteRenderer playerSpriteRenderer;



    private void Start()
    {
        moveSpeedHorizontal = 10f;
        basicGravity = 2f;
        isMoveable = true;

        playerRigidbody = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        playerRopeShooter = GetComponent<PlayerRopeShooter>();

        ResetPlayerMovement();
    }

    private void Update()
    {
        if(isMoveable)
        {
            // empty
        }

        if(playerRopeShooter.IsRopeShooted() && playerInput.scroll != 0)
        {
            MoveScroll(playerInput.scroll);
        }
    }

    private void MoveScroll(float inputSignal)
    {
        float direction = inputSignal > 0 ? 1 : -1;
        float adjustment = 20f;

        StunInAir();
        playerRigidbody.AddForce(adjustment * direction * Vector2.up, ForceMode2D.Impulse);
        playerRopeShooter.CutRope(Vector3.zero);

        ParticleManager.instance.PlayParticle(playerRocketParticlePrefab, transform.position);
        ParticleManager.instance.PlayParticleForSeconds(playerTrailParticlePrefab, gameObject, 0.5f);
    }

    public void SetPlayerGravity(float gravity)
    {
        playerRigidbody.gravityScale = gravity;
    }

    public void StunInAir()
    {
        playerRigidbody.velocity = Vector2.zero;
    }

    public void ResetPlayerMovement()
    {
        isMoveable = true;
        SetPlayerGravity(basicGravity);
    }




    // 캐릭터 자체 좌우 이동이 있을 때 만든 메서드
    private void MoveHorizontal(float inputSignal)
    {
        transform.Translate(moveSpeedHorizontal * inputSignal * Vector2.right * Time.deltaTime);
    }
    private void BrakeVelocityHorizontal()
    {
        float threshold = 0.1f;
        float brake = playerRigidbody.velocity.x < threshold ? 0.9f : 0f;

        playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x * brake, playerRigidbody.velocity.y);
    }
}
