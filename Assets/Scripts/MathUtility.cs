using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtility : MonoBehaviour
{
    public static Vector3 GetInternalDividingPoint(Vector3 pos1, Vector3 pos2, float ratio1, float ratio2)
    {
        return (ratio2 * pos1 + ratio1 * pos2) / (ratio1 + ratio2);
    }
}
