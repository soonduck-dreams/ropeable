using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : LivingThing
{
    [SerializeField] private GameObject playerDieParticleBlue;
    [SerializeField] private GameObject playerDieParticleYellow;
    [SerializeField] private GameObject playerFollower;

    private SpriteRenderer spriteRenderer;
    private PlayerRopeShooter playerRopeShooter;

    private float lastProtectTime = 0f;
    private float protectSeconds = 2f;
    private float fallToDeathThreshold = -0.5f;

    public enum State
    {
        Vulnerable, Protected
    }

    State state;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerRopeShooter = GetComponent<PlayerRopeShooter>();

        SetupHP(1, 1);

        state = State.Vulnerable;
    }

    private void Update()
    {
        if (GameManager.instance.gameState != GameManager.GameState.Playing)
        {
            return;
        }

        if (transform.position.y < fallToDeathThreshold)
        {
            Fall();
        }

        if (currentHP <= 0)
        {
            Die();
        }

        if(state == State.Protected && Time.time > lastProtectTime + protectSeconds)
        {
            state = State.Vulnerable;
        }

        AdjustGraphicDependingOnState();
    }

    public override void OnDamage(int damage)
    {
        if (state == State.Vulnerable)
        {
            base.OnDamage(damage);
            Protect();
        }
    }

    public override void OnForceDamage(int damage)
    {
        base.OnDamage(damage);
        Protect();
    }

    protected override void Die()
    {
        GameManager.instance.OnPlayerDie();
    }

    private void Fall()
    {
        OnForceDamage(maxHP);
    }

    private void Protect()
    {
        state = State.Protected;
        lastProtectTime = Time.time;
    }

    private void AdjustGraphicDependingOnState()
    {
        Color adjustedColor = spriteRenderer.color;

        switch (state)
        {
            case State.Vulnerable:
                adjustedColor.a = 1f;
                break;

            case State.Protected:
                adjustedColor.a = 0.5f;
                break;
        }

        spriteRenderer.color = adjustedColor;
    }

    public void PlayDieParticle()
    {
        ParticleManager.instance.PlayParticle(playerDieParticleBlue, transform.position);
        ParticleManager.instance.PlayParticle(playerDieParticleYellow, transform.position);
    }

    public void DestroyPlayer()
    {
        playerFollower.SetActive(false);
        playerRopeShooter.CutRope(Vector3.zero);
        AudioManager.instance.PlayEffects("Death", true);
        gameObject.SetActive(false);
    }
}