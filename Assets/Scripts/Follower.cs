using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField]
    private Transform targetTransform;

    [SerializeField]
    private float speed = 1f; // value between 0 ~ 1.

    private Vector3 curPosition;

    private void Start()
    {
        curPosition = targetTransform.position;
    }

    private void Update()
    {
        curPosition.x += (targetTransform.position.x - curPosition.x) * speed;
        curPosition.y += (targetTransform.position.y - curPosition.y) * speed;
        
        transform.position = curPosition;

        //RotateAlong();
    }

    private void RotateAlong()
    {
        transform.rotation = targetTransform.rotation;
    }
}
