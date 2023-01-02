using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    [SerializeField]
    private float halfCycleTime;

    [SerializeField]
    private float moveSpeed;

    private float currentTime;
    

    void Start()
    {
        currentTime = 0f;
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        if(currentTime < halfCycleTime)
        {
            transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
        }

        if(currentTime >= halfCycleTime * 2f)
        {
            currentTime = 0f;
        }
    }
}
