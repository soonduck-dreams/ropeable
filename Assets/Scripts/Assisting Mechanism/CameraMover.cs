using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField]
    private bool isXFixed = false;

    [SerializeField]
    private bool isYFixed = false;

    [SerializeField]
    private Rope rope;

    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private PlayerInput playerInput;

    public enum TargetMode
    {
        SectionSnapped,
        PlayerCentered
    }

    public TargetMode targetMode { get; private set; }

    private Vector3 targetPos;
    private Vector3 prevTargetPos;

    private Coroutine shakeCoroutine;

    private void Update()
    {
        SetTargetPosition();
        MoveCamera(0.1f);
    }

    private void SetTargetPosition()
    {
        float cameraPosZ = -10f;

        switch (GameManager.instance.gameState)
        {
            case GameManager.GameState.Playing:
            case GameManager.GameState.Paused:
            case GameManager.GameState.Gameover:
                SetTargetPositionWithSectionSnapped();
                break;

            case GameManager.GameState.Cleared:
                SetTargetPositionWithPlayerCentered();
                break;
        }

        LimitCameraMovementLeftsideAndDownside();
        targetPos.z = cameraPosZ;
    }

    private void MoveCamera(float speed)
    {
        float adjustedTargetPosX = isXFixed ? transform.position.x : targetPos.x;
        float adjustedTargetPosY = isYFixed ? transform.position.y : targetPos.y;

        Vector3 adjustedTargetPos = new Vector3(adjustedTargetPosX, adjustedTargetPosY, targetPos.z);

        transform.position += speed * (adjustedTargetPos - transform.position);
    }

    public bool isOutOfCamera(Vector3 pos, float threshold)
    {
        Vector3 checkPos = Camera.main.WorldToViewportPoint(pos);

        return
            checkPos.x < 0f - threshold || checkPos.x > 1f + threshold ||
            checkPos.y < 0f - threshold || checkPos.y > 1f + threshold;
    }

    public float ViewportDistanceToWorldDistance(Vector3 distance)
    {
        return (Camera.main.ViewportToWorldPoint(distance) - Camera.main.ViewportToWorldPoint(Vector3.zero)).magnitude;
    }

    public void LimitCameraMovementLeftsideAndDownside()
    {
        float downsidePosYLimit = 0f;
        float leftsidePosXLimit = 0f;

        if (targetPos.y - ViewportDistanceToWorldDistance(0.5f * Vector3.down) < downsidePosYLimit)
        {
            targetPos.y = downsidePosYLimit + ViewportDistanceToWorldDistance(new Vector3(0, 0.5f, 0));
        }

        if (targetPos.x - ViewportDistanceToWorldDistance(0.5f * Vector3.left) < leftsidePosXLimit)
        {
            targetPos.x = leftsidePosXLimit + ViewportDistanceToWorldDistance(new Vector3(0.5f, 0, 0));
        }
    }

    private void SetTargetPositionWithSectionSnapped()
    {
        float screenWidth = ViewportDistanceToWorldDistance(Vector3.right);
        float screenHeight = ViewportDistanceToWorldDistance(Vector3.up);

        float edgeRatio = 0f;
        float edgeWidth = screenWidth * edgeRatio;
        float edgeHeight = screenHeight * edgeRatio;

        int zoneNumX = (int)(playerTransform.position.x / (screenWidth - edgeWidth));
        int zoneNumY = (int)(playerTransform.position.y / (screenHeight - edgeHeight));

        bool isAtSectionX = playerTransform.position.x % (screenWidth - edgeWidth) >= edgeWidth;
        bool isAtSectionY = playerTransform.position.y % (screenHeight - edgeHeight) >= edgeHeight;

        if(isAtSectionX)
        {
            targetPos.x = ((2 * zoneNumX + 1) * (screenWidth - edgeWidth) + edgeWidth) / 2f;
        }
        else
        {
            targetPos.x = (2 * zoneNumX * (screenWidth - edgeWidth) + edgeWidth) / 2f;
        }

        if (isAtSectionY)
        {
            targetPos.y = ((2 * zoneNumY + 1) * (screenHeight - edgeHeight) + edgeHeight) / 2f;
        }
        else
        {
            targetPos.y = (2 * zoneNumY * (screenHeight - edgeHeight) + edgeHeight) / 2f;
        }

        targetMode = TargetMode.SectionSnapped;
    }

    private void SetTargetPositionWithPlayerCentered()
    {
        float playerFocusRatio = (1 / 3f) * (playerTransform.position - rope.ropeStartPoint).magnitude - (2 / 3f);
        // playerFocusRatio : 플레이어와 밧줄 반대편 지점 간 거리가 멀수록 카메라가 플레이어를 중심으로 잡도록 만드는 비율.
        // 거리가 20일 때 1 : 6 지점, 거리가 5일 때 1 : 1 지점에 카메라가 위치하도록 고안한 값

        if (rope.enabled)
        {
            targetPos = MathUtility.GetInternalDividingPoint(playerTransform.position, rope.ropeStartPoint, 1, playerFocusRatio);
        }
        else
        {
            targetPos = playerTransform.position;
        }

        targetMode = TargetMode.PlayerCentered;
    }

    public bool HasTargetPositionChanged()
    {
        if(prevTargetPos == null)
        {
            prevTargetPos = targetPos;
            return false;
        }

        if(prevTargetPos == targetPos)
        {
            return false;
        }

        prevTargetPos = targetPos;
        return true;
    }

    public void ShakeCamera(float seconds, float magnitude)
    {
        StartCoroutine(ShakeCameraCoroutine(seconds, magnitude, magnitude));
    }

    public void ShakeCamera(float seconds, float magnitudeX, float magnitudeY)
    {
        shakeCoroutine = StartCoroutine(ShakeCameraCoroutine(seconds, magnitudeX, magnitudeY));
    }

    private IEnumerator ShakeCameraCoroutine(float seconds, float magnitudeX, float magnitudeY)
    {
        TimeMeasurer timer = new TimeMeasurer();
        Vector3 originalPos;
        int shakeSign = 1;

        timer.StartMeasure();
        originalPos = transform.position;

        float randomMin = 0.5f; // 최소 shake 계수 (0.0 ~ 1.0)
        float randomX = Random.value;
        float randomY = Random.value;

        float shakeX = (randomX > randomMin ? randomX : randomMin) * magnitudeX * shakeSign;
        float shakeY = (randomY > randomMin ? randomY : randomMin) * magnitudeY * shakeSign;

        while (timer.CheckMeasure() < seconds)
        {
            if (HasTargetPositionChanged())
            {
                StopCoroutine(shakeCoroutine);
            }

            transform.position = originalPos + new Vector3(shakeX, shakeY, 0f);
            shakeSign *= -1;

            yield return null;
        }
    }
}
