using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtility : MonoBehaviour
{
    public static Vector3 GetInternalDividingPoint(Vector3 pos1, Vector3 pos2, float ratio1, float ratio2)
    {
        return (ratio2 * pos1 + ratio1 * pos2) / (ratio1 + ratio2);
    }

    public static float GetAngleBetweenVectors(Vector3 v1, Vector3 v2)
    {
        // v1을 기준으로 v2의 각도 반환 (0 <= return < 360, 시계 반대 방향으로 각도 증가)
        float angle = Vector3.Angle(v1, v2);
        return Vector3.Dot(Vector3.back, Vector3.Cross(v2, v1)) > 0 ? angle : 360 - angle;
    }
}
