using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MagnetBlock : MonoBehaviour, IRopeable, IMagnet
{
    public enum MagnetState
    {
        None,
        Pull,
        Push
    }

    public enum MagnetMechanism
    {
        RopePowered,
        HangSwitch,
        CutSwitch
    }

    private MagnetState magnetState;
    public MagnetState offState;
    public MagnetState onState;

    public MagnetMechanism mechanism;

    private float radiusAdjustment = 50f;
    public float pullAdjustment;
    public float pushAdjustment;

    [SerializeField] private CameraMover cameraMover;

    [SerializeField] private Sprite noneSprite;
    [SerializeField] private Sprite pullSprite;
    [SerializeField] private Sprite pushSprite;

    private Vector2 positionAdjustment = new Vector2(1f, -0.4f); // hard-coding으로 찾아낸 보정 값
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        SetState(offState);
    }

    private void FixedUpdate()
    {
        if (cameraMover.isOutOfCamera(transform.position, 0f))
        {
            return;
        }

        switch (magnetState)
        {
            case MagnetState.None:
                break;

            case MagnetState.Pull:
                PullMagnetTargets();
                break;

            case MagnetState.Push:
                PushMagnetTargets();
                break;
        }
    }

    private void SetState(MagnetState state)
    {
        magnetState = state;
        SetSprite();
    }

    public string GetTerrainType()
    {
        return "magnet";
    }

    public void OnRopeHang()
    {
        switch (mechanism)
        {
            case MagnetMechanism.RopePowered:
                SetState(onState);
                break;

            case MagnetMechanism.HangSwitch:
                SetState(magnetState == onState ? offState : onState);
                break;

            case MagnetMechanism.CutSwitch:
                break;
        }
    }

    public void OnRopeCut()
    {
        switch (mechanism)
        {
            case MagnetMechanism.RopePowered:
                SetState(offState);
                break;

            case MagnetMechanism.HangSwitch:
                break;

            case MagnetMechanism.CutSwitch:
                SetState(magnetState == onState ? offState : onState);
                break;
        }
    }

    public void PullMagnetTargets()
    {
        List<Rigidbody2D> targets = GetMagnetTargets();

        foreach (var target in targets)
        {
            Vector2 directionVector = ((Vector2)transform.position + positionAdjustment - target.position).normalized;
            float distanceAdjustment = 5f / ((Vector2)transform.position + positionAdjustment - target.position).magnitude;

            target.AddForce(pullAdjustment * distanceAdjustment * directionVector);
        }
    }

    public void PushMagnetTargets()
    {
        List<Rigidbody2D> targets = GetMagnetTargets();

        foreach (var target in targets)
        {
            Vector2 directionVector = (target.position - (Vector2)transform.position).normalized;
            float distanceAdjustment = 5f / ((Vector2)transform.position + positionAdjustment - target.position).magnitude;


            target.AddForce(pushAdjustment * distanceAdjustment * directionVector);
        }
    }

    private List<Rigidbody2D> GetMagnetTargets()
    {
        Collider2D[] colliders = new Collider2D[20];
        ContactFilter2D filter = new ContactFilter2D();

        filter.SetDepth(0f, 0f);
        filter.useDepth = true;

        Physics2D.OverlapCircle(transform.position, radiusAdjustment, filter, colliders);

        List<Collider2D> collidersList = colliders.OfType<Collider2D>().ToList();
        List<Rigidbody2D> targets = new List<Rigidbody2D>();

        foreach (var collider in collidersList)
        {
            IMagnetTarget magnetTarget = collider.GetComponent<IMagnetTarget>();
            Rigidbody2D targetRigidbody = collider.GetComponent<Rigidbody2D>();

            if (magnetTarget != null && targetRigidbody != null)
            {
                targets.Add(collider.GetComponent<Rigidbody2D>());
            }
        }

        return targets;
    }

    private void SetSprite()
    {
        switch (magnetState)
        {
            case MagnetState.None:
                spriteRenderer.sprite = noneSprite;
                break;

            case MagnetState.Pull:
                spriteRenderer.sprite = pullSprite;
                break;

            case MagnetState.Push:
                spriteRenderer.sprite = pushSprite;
                break;
        }
    }
}
