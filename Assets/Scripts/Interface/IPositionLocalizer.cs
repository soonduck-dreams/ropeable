using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPositionLocalizer
{
    Vector3 WorldToLocalPos(Vector3 worldPos);
    Vector3 LocalToWorldPos(Vector3 localPos);
    bool IsPulledByRope();
    GameObject GetGameObject();
}